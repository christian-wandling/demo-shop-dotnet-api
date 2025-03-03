#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Commands.ConvertShoppingSessionToOrder;

public sealed class ConvertShoppingSessionToOrderCommandHandler(
    IOrderRepository repository,
    IDomainEventDispatcher eventDispatcher,
    ILogger logger
)
    : IRequestHandler<ConvertShoppingSessionToOrderCommand, Result<OrderEntity>>
{
    public async Task<Result<OrderEntity>> Handle(ConvertShoppingSessionToOrderCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Session, nameof(request.Session));

        try
        {
            LogCommandStarted(logger, request.Session.Id, request.Session.UserId);

            var unsavedResult = request.Session.ConvertToOrder();

            if (!unsavedResult.IsSuccess)
                return unsavedResult.Map();

            var savedResult = await SaveChanges(unsavedResult.Value, cancellationToken);

            if (!savedResult.IsSuccess)
            {
                LogCommandError(logger, request.Session.Id);
                return savedResult.Map();
            }

            LogCommandSuccess(logger, request.Session.Id, savedResult.Value.Id);
            return savedResult;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogOperationFailed("Convert shopping session to order", "ShoppingSessionId", $"{request.Session.Id}",
                ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<Result<OrderEntity>> SaveChanges(OrderEntity unsavedOrder, CancellationToken cancellationToken)
    {
        var savedOrder = await repository.CreateOrderAsync(unsavedOrder, cancellationToken);

        if (savedOrder is null)
            return Result.Error("Failed to create order");

        await eventDispatcher.DispatchEventsAsync(unsavedOrder, cancellationToken);

        return Result.Success(savedOrder);
    }

    private static void LogCommandStarted(ILogger logger, int sessionId, int userId) =>
        logger.ForContext("EventId", LoggerEventIds.ConvertShoppingSessionToOrderCommandStarted)
            .Information("Starting to convert shopping session with ID {SessionId} for user {UserId}",
                sessionId, userId);

    private static void LogCommandSuccess(ILogger logger, int sessionId, int orderId) =>
        logger.ForContext("EventId", LoggerEventIds.ConvertShoppingSessionToOrderCommandSuccess)
            .Information("Successfully completed converting shopping session with ID {SessionId} to order {OrderId}",
                sessionId, orderId);

    private static void LogCommandError(ILogger logger, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.ConvertShoppingSessionToOrderCommandError)
            .Information("Error converting shopping session with ID {SessionId} to order",
                sessionId);
}
