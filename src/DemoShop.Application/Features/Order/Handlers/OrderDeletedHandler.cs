#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Handlers;

public class OrderDeletedHandler(ILogger logger)
    : INotificationHandler<OrderDeletedDomainEvent>
{
    public Task Handle(OrderDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogOrderDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogOrderDeleted(ILogger logger, int id) => logger.Information(
        "Order deleted: {Id} {@EventId}", id, LoggerEventIds.OrderDeletedDomainEvent);
}
