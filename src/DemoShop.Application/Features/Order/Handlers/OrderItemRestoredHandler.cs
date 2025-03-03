#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Handlers;

public class OrderItemRestoredHandler(ILogger logger)
    : INotificationHandler<OrderItemRestoredDomainEvent>
{
    public Task Handle(OrderItemRestoredDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogOrderItemRestored(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogOrderItemRestored(ILogger logger, int id) => logger.Information(
        "OrderItem restored: {Id} {@EventId}", id, LoggerEventIds.OrderItemRestoredDomainEvent);
}
