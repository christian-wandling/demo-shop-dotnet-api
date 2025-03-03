#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Features.Products;

public class ProductRepository(ApplicationDbContext context, ILogger logger) : IProductRepository
{
    public async Task<ProductEntity?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(id, nameof(id));
        LogGetProductByIdStarted(logger, id);

        var result = await context.Query<ProductEntity>()
            .Include(p => p.Categories)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (result is null)
            LogGetProductByIdNotFound(logger, id);
        else
            LogGetProductByIdCompleted(logger, id);

        return result;
    }

    public async Task<IEnumerable<ProductEntity>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        LogGetAllProductsStarted(logger);

        var result = await context.Query<ProductEntity>()
            .Include(p => p.Categories)
            .Include(p => p.Images)
            .ToListAsync(cancellationToken);

        LogGetAllProductsCompleted(logger, result.Count);

        return result;
    }

    private static void LogGetAllProductsStarted(ILogger logger) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetAllProductsStarted)
            .Debug("Getting all products started");

    private static void LogGetAllProductsCompleted(ILogger logger, int productCount) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetAllProductsSuccess)
            .Debug("Getting all products completed. Retrieved {ProductCount} products successfully", productCount);

    private static void LogGetProductByIdStarted(ILogger logger, int productId) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetProductByIdStarted)
            .Debug("Attempting to get Product with ID {ProductId}", productId);

    private static void LogGetProductByIdCompleted(ILogger logger, int productId) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetProductByIdSuccess)
            .Debug("Attempting to get product with ID {ProductId} completed successfully", productId);

    private static void LogGetProductByIdNotFound(ILogger logger, int productId) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetProductByIdNotFound)
            .Debug("Product with ID {ProductId} not found in database", productId);
}
