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

public class ProductDeletedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<ProductDeletedDomainEvent>
{
    public Task Handle(ProductDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        InvalidateCache(notification.Id);
        LogProductDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private void InvalidateCache(int id)
    {
        var cacheKeyProduct = cacheService.GenerateCacheKey("product", new GetProductByIdQuery(id));
        cacheService.InvalidateCache(cacheKeyProduct);

        var cacheKeyAllProducts = cacheService.GenerateCacheKey("product", new GetAllProductsQuery());
        cacheService.InvalidateCache(cacheKeyAllProducts);
    }

    private static void LogProductDeleted(ILogger logger, int id) => logger.Information(
        "Product deleted: {Id} {@EventId}", id, LoggerEventIds.ProductDeletedDomainEvent);
}
