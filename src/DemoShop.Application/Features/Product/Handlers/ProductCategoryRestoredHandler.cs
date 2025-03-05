#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductCategoryRestoredHandler(ILogger logger)
    : INotificationHandler<ProductCategoryRestoredDomainEvent>
{
    public Task Handle(ProductCategoryRestoredDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogProductCategoryRestored(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogProductCategoryRestored(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.ProductCategoryRestoredDomainEvent)
        .Information("Product category restored: {Id}", id);
}
