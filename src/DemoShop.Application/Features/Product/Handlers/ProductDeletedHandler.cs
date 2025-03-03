#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductDeletedHandler(ILogger logger)
    : INotificationHandler<ProductDeletedDomainEvent>
{
    public Task Handle(ProductDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogProductDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogProductDeleted(ILogger logger, int id) => logger.Information(
        "Product deleted: {Id} {@EventId}", id, LoggerEventIds.ProductDeletedDomainEvent);
}
