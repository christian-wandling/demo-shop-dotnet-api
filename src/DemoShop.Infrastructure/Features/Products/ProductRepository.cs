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
            .AsNoTracking()
            .Include(p => p.Categories)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (result is null)
            LogGetProductByIdNotFound(logger, id);
        else
            LogGetProductByIdSuccess(logger, id);

        return result;
    }

    public async Task<IEnumerable<ProductEntity>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        LogGetAllProductsStarted(logger);

        var result = await context.Query<ProductEntity>()
            .AsNoTracking()
            .Include(p => p.Categories)
            .Include(p => p.Images)
            .ToListAsync(cancellationToken);

        LogGetAllProductsSuccess(logger, result.Count);

        return result;
    }

    private static void LogGetAllProductsStarted(ILogger logger) =>
        logger
            .ForContext("EventId", LoggerEventId.GetAllProductsStarted)
            .Debug("Getting all products started");

    private static void LogGetAllProductsSuccess(ILogger logger, int count) =>
        logger
            .ForContext("EventId", LoggerEventId.GetAllProductsSuccess)
            .Debug("Getting all products completed. Retrieved {Count} products successfully", count);

    private static void LogGetProductByIdStarted(ILogger logger, int id) =>
        logger
            .ForContext("EventId", LoggerEventId.GetProductByIdStarted)
            .Debug("Attempting to get Product with ID {Id}", id);

    private static void LogGetProductByIdSuccess(ILogger logger, int id) =>
        logger
            .ForContext("EventId", LoggerEventId.GetProductByIdSuccess)
            .Debug("Attempting to get product with ID {Id} completed successfully", id);

    private static void LogGetProductByIdNotFound(ILogger logger, int id) =>
        logger
            .ForContext("EventId", LoggerEventId.GetProductByIdNotFound)
            .Warning("Product with ID {Id} not found in database", id);
}
