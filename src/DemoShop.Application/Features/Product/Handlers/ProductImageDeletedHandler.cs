#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductImageDeletedHandler(ILogger logger)
    : INotificationHandler<ImageDeletedDomainEvent>
{
    public Task Handle(ImageDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogProductImageDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogProductImageDeleted(ILogger logger, int id) => logger.Information(
        "Product image deleted: {Id} {@EventId}", id, LoggerEventIds.ProductImageDeletedDomainEvent);
}
