#region

using Ardalis.GuardClauses;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Product.Queries.GetAllProducts;
using DemoShop.Application.Features.Product.Queries.GetProductById;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductCategoryCreatedHandler(ILogger logger)
    : INotificationHandler<ProductCategoryCreatedDomainEvent>
{
    public Task Handle(ProductCategoryCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.Category, nameof(notification.Category));

        LogCategoryCreated(logger, notification.Category.Id);
        return Task.CompletedTask;
    }

    private static void LogCategoryCreated(ILogger logger, int id) => logger.Information(
        "Product category created: {Id} {@EventId}", id, LoggerEventIds.ProductCategoryCreatedDomainEvent);
}
