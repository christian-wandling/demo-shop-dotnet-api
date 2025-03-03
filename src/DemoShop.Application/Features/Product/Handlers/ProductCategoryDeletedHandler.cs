#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductCategoryDeletedHandler(ILogger logger)
    : INotificationHandler<ProductCategoryDeletedDomainEvent>
{
    public Task Handle(ProductCategoryDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogProductCategoryDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogProductCategoryDeleted(ILogger logger, int id) => logger.Information(
        "Product category deleted: {Id} {@EventId}", id, LoggerEventIds.ProductCategoryDeletedDomainEvent);
}
