#region

using Ardalis.GuardClauses;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Product.Queries.GetAllProducts;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductCreatedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<ProductCreatedDomainEvent>
{
    public Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        InvalidateCache();
        LogProductCreated(logger, notification.Id);
        return Task.CompletedTask;
    }

    private void InvalidateCache()
    {
        var cacheKeyAllProducts = cacheService.GenerateCacheKey("product", new GetAllProductsQuery());
        cacheService.InvalidateCache(cacheKeyAllProducts);
    }

    private static void LogProductCreated(ILogger logger, int id) => logger.Information(
        "Product created: {Id} {@EventId}", id, LoggerEventIds.ProductCreatedDomainEvent);
}
