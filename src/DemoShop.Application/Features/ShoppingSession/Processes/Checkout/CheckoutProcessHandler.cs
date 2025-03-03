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
using Serilog;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Processes.Checkout;

public sealed class CheckoutProcessHandler(
    ICurrentShoppingSessionAccessor currentSession,
    IMapper mapper,
    IMediator mediator,
    ILogger logger,
    IDomainEventDispatcher eventDispatcher,
    IUnitOfWork unitOfWork
)
    : IRequestHandler<CheckoutProcess, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CheckoutProcess request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var sessionResult = await currentSession.GetCurrent(cancellationToken);

        if (!sessionResult.IsSuccess) return Result.NotFound("No active session found");

        using (unitOfWork)
        {
            try
            {
                LogProcessStarted(logger, sessionResult.Value.UserId, sessionResult.Value.Id);
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var savedOrderResult = await ConvertShoppingSessionToOrder(sessionResult.Value, cancellationToken);

                if (!savedOrderResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    LogProcessFailed(logger, savedOrderResult.Value.UserId, savedOrderResult.Value.Id);
                    return savedOrderResult.Map();
                }

                var deleteSessionResult = await DeleteShoppingSession(sessionResult.Value, cancellationToken);

                if (!deleteSessionResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    LogProcessFailed(logger, savedOrderResult.Value.UserId, savedOrderResult.Value.Id);
                    return deleteSessionResult.Map();
                }

                var savedResult = await SaveChanges(savedOrderResult.Value, cancellationToken);

                if (!savedResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    LogProcessFailed(logger, savedOrderResult.Value.UserId, savedOrderResult.Value.Id);
                    return savedResult.Map();
                }

                LogProcessSuccess(logger, savedOrderResult.Value.UserId, savedOrderResult.Value.Id,
                    sessionResult.Value.Id);
                return Result.Success(mapper.Map<OrderResponse>(savedResult.Value));
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
        var command = new CreateOrderCommand(session);
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

    private static void LogProcessStarted(ILogger logger, int userId, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.CheckoutProcessStarted)
            .Information("Starting checkout user {UserId} for shopping session {SessionId}",
                sessionId, userId);

    private static void LogProcessSuccess(ILogger logger, int userId, int orderId, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.CheckoutProcessSuccess)
            .Information(
                "Successfully completed checkout for {UserId} - Order {OrderId} was created from shopping session {SessionId}",
                userId, orderId, sessionId);

    private static void LogProcessFailed(ILogger logger, int userId, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.CheckoutProcessFailed)
            .Information("Error while checkout user {UserId} for shopping session {SessionId}",
                userId, sessionId);
}
