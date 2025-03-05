#region

using Ardalis.GuardClauses;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Handlers;

public class OrderCreatedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<OrderCreatedDomainEvent>
{
    public Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        var cacheKey = cacheService.GenerateCacheKey("orders-of-user", notification.UserId);
        cacheService.InvalidateCache(cacheKey);

        LogOrderCreated(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogOrderCreated(ILogger logger, int id) => logger.Information(
        "Order created: {Id} {@EventId}", id, LoggerEventIds.OrderCreatedDomainEvent);
}
