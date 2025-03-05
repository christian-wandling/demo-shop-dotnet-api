#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Handlers;

public class OrderRestoredHandler(ILogger logger)
    : INotificationHandler<OrderRestoredDomainEvent>
{
    public Task Handle(OrderRestoredDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogOrderRestored(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogOrderRestored(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.OrderRestoredDomainEvent)
        .Information("Order restored: {Id}", id);
}
