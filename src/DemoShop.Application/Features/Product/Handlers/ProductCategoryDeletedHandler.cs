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

public class ProductCategoryDeletedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<ProductCategoryDeletedDomainEvent>
{
    public Task Handle(ProductCategoryDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        InvalidateCache(notification.ProductIds);
        LogProductCategoryDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private void InvalidateCache(IReadOnlyCollection<int> productIds)
    {
        var cacheKeyAllProducts = cacheService.GenerateCacheKey("product", new GetAllProductsQuery());
        cacheService.InvalidateCache(cacheKeyAllProducts);

        foreach (var productId in productIds)
        {
            var cacheKeyProduct = cacheService.GenerateCacheKey("product", new GetProductByIdQuery(productId));
            cacheService.InvalidateCache(cacheKeyProduct);
        }
    }

    private static void LogProductCategoryDeleted(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.ProductCategoryDeletedDomainEvent)
        .Information("Product category deleted: {Id}", id);
}
