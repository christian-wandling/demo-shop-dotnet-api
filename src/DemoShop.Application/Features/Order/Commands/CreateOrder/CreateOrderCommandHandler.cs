#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Order.Commands.ConvertShoppingSessionToOrder;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.ShoppingSession.Commands.DeleteShoppingSession;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.ShoppingSession.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.Order.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    ICurrentShoppingSessionAccessor currentSession,
    IMapper mapper,
    IMediator mediator,
    ILogger<CreateOrderCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IUnitOfWork unitOfWork
)
    : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var sessionResult = await currentSession.GetCurrent(cancellationToken);

        if (!sessionResult.IsSuccess) return Result.NotFound("No active session found");

        using (unitOfWork)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var savedOrderResult = await ConvertShoppingSessionToOrder(sessionResult.Value, cancellationToken);

                if (!savedOrderResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return savedOrderResult.Map();
                }

                var deleteSessionResult = await DeleteShoppingSession(sessionResult.Value, cancellationToken);

                if (!deleteSessionResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return deleteSessionResult.Map();
                }

                var savedResult = await SaveChanges(savedOrderResult.Value, cancellationToken);

                return savedResult.IsSuccess
                    ? Result.Success(mapper.Map<OrderResponse>(savedResult.Value))
                    : savedResult.Map();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogDomainException(ex.Message);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Error(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                logger.LogOperationFailed("Create Order from shopping session", "ShoppingSessionId",
                    $"{sessionResult.Value.Id}", ex);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Error("Failed to create ShoppingSession");
            }
            finally
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
            }
        }
    }

    private async Task<Result<OrderEntity>> ConvertShoppingSessionToOrder(ShoppingSessionEntity session,
        CancellationToken cancellationToken)
    {
        var command = new ConvertShoppingSessionToOrderCommand(session);
        return await mediator.Send(command, cancellationToken);
    }

    private async Task<Result> DeleteShoppingSession(ShoppingSessionEntity session,
        CancellationToken cancellationToken)
    {
        var command = new DeleteShoppingSessionCommand(session);
        return await mediator.Send(command, cancellationToken);
    }

    private async Task<Result<OrderEntity>> SaveChanges(OrderEntity savedOrder, CancellationToken cancellationToken)
    {
        await unitOfWork.CommitTransactionAsync(cancellationToken);
        await eventDispatcher.DispatchEventsAsync(savedOrder, cancellationToken);

        return Result.Success(savedOrder);
    }
}
