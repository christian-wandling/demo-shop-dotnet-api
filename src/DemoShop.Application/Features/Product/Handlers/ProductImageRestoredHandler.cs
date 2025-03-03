#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductImageRestoredHandler(ILogger logger)
    : INotificationHandler<ImageRestoredDomainEvent>
{
    public Task Handle(ImageRestoredDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogProductImageRestored(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogProductImageRestored(ILogger logger, int id) => logger.Information(
        "Product image restored: {Id} {@EventId}", id, LoggerEventIds.ProductImageRestoredDomainEvent);
}
