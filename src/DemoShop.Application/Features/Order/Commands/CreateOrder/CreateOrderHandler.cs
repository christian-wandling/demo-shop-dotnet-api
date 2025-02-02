using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.Order.Commands.ConvertShoppingSessionToOrder;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.ShoppingSession.Commands.DeleteShoppingSession;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Order.Commands.CreateOrder;

public sealed class CreateOrderHandler(
    ICurrentShoppingSessionAccessor session,
    IMapper mapper,
    IMediator mediator,
    ILogger<CreateOrderHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IUnitOfWork unitOfWork
)
    : IRequestHandler<CreateOrderCommand, Result<OrderResponse?>>
{
    public async Task<Result<OrderResponse?>> Handle(CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var sessionResult = await session.GetCurrent(cancellationToken).ConfigureAwait(false);

        if (!sessionResult.IsSuccess) return Result.NotFound("No active session found");

        using (unitOfWork)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

                var convertShoppingSessionToOrderCommand = new ConvertShoppingSessionToOrderCommand(sessionResult.Value);
                var convertShoppingSessionToOrderResult = await mediator
                    .Send(convertShoppingSessionToOrderCommand, cancellationToken).ConfigureAwait(false);
                //
                // if (!convertShoppingSessionToOrderResult.IsSuccess)
                // {
                //     throw;
                // }
                //
                // if (convertShoppingSessionToOrderResult.Value == null)
                // {
                //     throw;
                // }
                Guard.Against.Null(convertShoppingSessionToOrderResult.Value,
                    nameof(convertShoppingSessionToOrderResult.IsSuccess));

                var deleteSessionCommand = new DeleteShoppingSessionCommand(sessionResult.Value);
                var deleteSessionResult =
                    await mediator.Send(deleteSessionCommand, cancellationToken).ConfigureAwait(false);

                // if (!deleteSessionResult.IsSuccess)
                // {
                //     throw;
                // }

                await unitOfWork.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                await eventDispatcher.DispatchEventsAsync(convertShoppingSessionToOrderResult.Value, cancellationToken)
                    .ConfigureAwait(false);
                return Result.Success(mapper.Map<OrderResponse?>(convertShoppingSessionToOrderResult.Value));
            }
            catch (NullReferenceException ex)
            {
                logger.LogOperationFailed("Create Order from shopping session", "ShoppingSessionId",
                    $"{sessionResult.Value.Id}", ex);
                await unitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                return Result.Error("Failed to create ShoppingSession");
            }
        }
    }
}
