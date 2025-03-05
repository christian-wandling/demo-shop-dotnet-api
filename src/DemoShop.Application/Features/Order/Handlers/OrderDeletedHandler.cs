#region

using Ardalis.GuardClauses;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Order.Queries.GetOrderById;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Handlers;

public class OrderDeletedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<OrderDeletedDomainEvent>
{
    public Task Handle(OrderDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        InvalidateCache(notification.Id, notification.UserId);
        LogOrderDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private void InvalidateCache(int id, int userId)
    {
        var cacheKeyOrder = cacheService.GenerateCacheKey("order", new GetOrderByIdQuery(id));
        cacheService.InvalidateCache(cacheKeyOrder);
        var cacheKeyOrdersOfUser = cacheService.GenerateCacheKey("orders-of-user", userId);
        cacheService.InvalidateCache(cacheKeyOrdersOfUser);
    }

    private static void LogOrderDeleted(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.OrderDeletedDomainEvent)
        .Information("Order deleted: {Id}", id);
}
