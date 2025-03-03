#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Commands.ConvertShoppingSessionToOrder;

public sealed class CreateOrderCommandHandler(
    IOrderRepository repository,
    IDomainEventDispatcher eventDispatcher,
    ILogger logger,
    ICacheService cacheService
)
    : IRequestHandler<CreateOrderCommand, Result<OrderEntity>>
{
    public async Task<Result<OrderEntity>> Handle(CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Session, nameof(request.Session));

        try
        {
            LogCommandStarted(logger, request.Session.Id);

            var unsavedResult = request.Session.ConvertToOrder();

            if (!unsavedResult.IsSuccess)
                return unsavedResult.Map();

            var savedResult = await SaveChanges(unsavedResult.Value, cancellationToken);

            if (!savedResult.IsSuccess)
            {
                LogCommandError(logger, request.Session.Id);
                return savedResult.Map();
            }

            InvalidateOrdersCache();
            LogCommandSuccess(logger, request.Session.Id, savedResult.Value.Id);
            return savedResult;
        }
        catch (InvalidOperationException ex)
        {
            LogInvalidOperationException(logger, ex.Message, ex);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            LogDatabaseException(logger, ex.Message, ex);
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

    private void InvalidateOrdersCache()
    {
        var cacheKey = cacheService.GenerateCacheKey("order", new GetAllOrdersOfUserQuery());
        cacheService.InvalidateCache(cacheKey);
    }

    private static void LogCommandStarted(ILogger logger, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.CreateOrderCommandStarted)
            .Information("Starting to create order from shopping session with ID {SessionId}",
                sessionId);

    private static void LogCommandSuccess(ILogger logger, int orderId, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.CreateOrderCommandSuccess)
            .Information("Successfully created order with Id {OrderId} from shopping session {SessionId}",
                orderId, sessionId);

    private static void LogCommandError(ILogger logger, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.CreateOrderCommandError)
            .Information("Error creating order from shopping session with ID {SessionId}",
                sessionId);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) =>
        logger.Error(ex, "Database error occurred while creating order. Error: {ErrorMessage} {@EventId}",
            errorMessage, LoggerEventIds.CreateOrderDatabaseException);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) =>
        logger.Error(ex, "Invalid operation while creating order. Error: {ErrorMessage} {@EventId}",
            errorMessage, LoggerEventIds.CreateOrderDomainException);
}
