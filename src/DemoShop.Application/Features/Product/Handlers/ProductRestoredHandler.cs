#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductRestoredHandler(ILogger logger)
    : INotificationHandler<ProductRestoredDomainEvent>
{
    public Task Handle(ProductRestoredDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogProductRestored(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogProductRestored(ILogger logger, int id) => logger.Information(
        "Product restored: {Id} {@EventId}", id, LoggerEventIds.ProductRestoredDomainEvent);
}
