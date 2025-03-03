#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Handlers;

public class OrderCreatedHandler(ILogger logger)
    : INotificationHandler<OrderCreatedDomainEvent>
{
    public Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.Order, nameof(notification.Order));

        LogOrderCreated(logger, notification.Order.Id);
        return Task.CompletedTask;
    }

    private static void LogOrderCreated(ILogger logger, int id) => logger.Information(
        "Order created: {Id} {@EventId}", id, LoggerEventIds.OrderCreatedDomainEvent);
}
