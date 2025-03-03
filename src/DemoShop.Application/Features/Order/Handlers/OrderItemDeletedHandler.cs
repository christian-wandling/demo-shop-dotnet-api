#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Handlers;

public class OrderItemDeletedHandler(ILogger logger)
    : INotificationHandler<OrderItemDeletedDomainEvent>
{
    public Task Handle(OrderItemDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogOrderItemDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogOrderItemDeleted(ILogger logger, int id) => logger.Information(
        "OrderItem deleted: {Id} {@EventId}", id, LoggerEventIds.OrderItemDeletedDomainEvent);
}
