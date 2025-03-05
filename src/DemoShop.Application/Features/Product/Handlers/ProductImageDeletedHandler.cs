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

public class ProductImageDeletedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<ImageDeletedDomainEvent>
{
    public Task Handle(ImageDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        InvalidateCache(notification.ProductId);
        LogProductImageDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private void InvalidateCache(int productId)
    {
        var cacheKeyAllProducts = cacheService.GenerateCacheKey("product", new GetAllProductsQuery());
        cacheService.InvalidateCache(cacheKeyAllProducts);
        var cacheKeyProduct = cacheService.GenerateCacheKey("product", new GetProductByIdQuery(productId));
        cacheService.InvalidateCache(cacheKeyProduct);
    }

    private static void LogProductImageDeleted(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.ProductImageDeletedDomainEvent)
        .Information("Product image deleted: {Id}", id);
}
