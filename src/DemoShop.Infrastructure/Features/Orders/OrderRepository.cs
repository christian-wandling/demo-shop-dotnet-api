#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Features.Orders;

public class OrderRepository(ApplicationDbContext context, ILogger logger) : IOrderRepository
{
    public async Task<OrderEntity?> GetOrderByIdAsync(int orderId, int userId, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(orderId, nameof(orderId));
        Guard.Against.NegativeOrZero(userId, nameof(userId));

        LogGetOrderByIdStarted(logger, orderId, userId);

        var result = await context.Query<OrderEntity>()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId, cancellationToken);

        if (result is null)
            LogGetOrderByIdNotFound(logger, orderId, userId);
        else
            LogGetOrderByIdSuccess(logger, orderId, userId);

        return result;
    }

    public async Task<IEnumerable<OrderEntity>> GetOrdersByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(userId, nameof(userId));

        LogGetOrdersByUserIdStarted(logger, userId);

        var result = await context.Query<OrderEntity>()
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .ToListAsync(cancellationToken);

        LogGetOrdersByUserIdSuccess(logger, userId, result.Count);

        return result;
    }

    public async Task<OrderEntity?> CreateOrderAsync(OrderEntity orderEntity, CancellationToken cancellationToken)
    {
        Guard.Against.Null(orderEntity, nameof(orderEntity));

        LogCreateOrderStarted(logger, orderEntity.UserId);

        var created = context.Set<OrderEntity>().Add(orderEntity);
        await context.SaveChangesAsync(cancellationToken);

        LogCreateOrderSuccess(logger, created.Entity.Id, orderEntity.UserId);

        return created.Entity;
    }

    private static void LogGetOrdersByUserIdStarted(ILogger logger, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.GetOrdersByUserIdStarted)
            .Debug("Getting all orders for user {UserId} started", userId);

    private static void LogGetOrdersByUserIdSuccess(ILogger logger, int userId, int count) =>
        logger
            .ForContext("EventId", LoggerEventId.GetOrdersByUserIdSuccess)
            .Debug("Getting all orders for user {UserId} completed. Retrieved {Count} products successfully", userId,
                count);

    private static void LogGetOrderByIdStarted(ILogger logger, int orderId, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.GetOrderByIdStarted)
            .Debug("Attempting to get order with Id {OrderId} for user {UserId}", orderId, userId);

    private static void LogGetOrderByIdSuccess(ILogger logger, int orderId, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.GetOrderByIdSuccess)
            .Debug("Attempting to get order with ID {OrderId} for user {UserId} completed successfully", orderId,
                userId);

    private static void LogGetOrderByIdNotFound(ILogger logger, int orderId, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.GetOrderByIdNotFound)
            .Warning("Order with ID {OderId} for user {UserId} not found in database", orderId, userId);

    private static void LogCreateOrderStarted(ILogger logger, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.CreateOrderStarted)
            .Debug("Attempting to create order for user {UserId}", userId);

    private static void LogCreateOrderSuccess(ILogger logger, int orderId, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.CreateOrderSuccess)
            .Debug("Attempting to create order with ID {OrderId} for user {UserId} completed successfully", orderId,
                userId);
}
