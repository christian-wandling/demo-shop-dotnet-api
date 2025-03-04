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

public class ProductRestoredHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<ProductRestoredDomainEvent>
{
    public Task Handle(ProductRestoredDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        InvalidateCache(notification.Id);
        LogProductRestored(logger, notification.Id);
        return Task.CompletedTask;
    }

    private void InvalidateCache(int id)
    {
        var cacheKeyProduct = cacheService.GenerateCacheKey("product", new GetProductByIdQuery(id));
        cacheService.InvalidateCache(cacheKeyProduct);

        var cacheKeyAllProducts = cacheService.GenerateCacheKey("product", new GetAllProductsQuery());
        cacheService.InvalidateCache(cacheKeyAllProducts);
    }

    private static void LogProductRestored(ILogger logger, int id) => logger.Information(
        "Product restored: {Id} {@EventId}", id, LoggerEventIds.ProductRestoredDomainEvent);
}
