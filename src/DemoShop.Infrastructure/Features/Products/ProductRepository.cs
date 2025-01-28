using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.Interfaces;
using DemoShop.Infrastructure.Common;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DemoShop.Infrastructure.Features.Products;

public class ProductRepository(ApplicationDbContext context, ILogger<ProductRepository> logger)
    : Repository<ProductEntity>(context, logger), IProductRepository
{
    public async Task<Result<ProductEntity>> GetProductByIdAsync(int id)
    {
        Guard.Against.NegativeOrZero(id, nameof(id));

        try
        {
            var product = await context.Query<ProductEntity>()
                .AsNoTracking()
                .Include(p => p.Categories)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            return product is not null
                ? Result<ProductEntity>.Success(product)
                : HandleNotFound<ProductEntity>("GetProductByIdAsync", $"{id}");
        }
        catch (DbException ex)
        {
            return HandleDbException<ProductEntity>("GetProductByIdAsync", ex, $"{id}");
        }
        catch (InvalidOperationException ex)
        {
            return HandleInvalidOperationException<ProductEntity>("GetProductByIdAsync", ex, $"{id}");
        }
    }

    public async Task<Result<IEnumerable<ProductEntity>>> GetAllProductsAsync()
    {
        try
        {
            return await context.Query<ProductEntity>()
                .AsNoTracking()
                .Include(p => p.Categories)
                .Include(p => p.Images)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (DbException ex)
        {
            return HandleDbException<IEnumerable<ProductEntity>>("GetAllProductsAsync", ex);
        }
        catch (InvalidOperationException ex)
        {
            return HandleInvalidOperationException<IEnumerable<ProductEntity>>("GetAllProductsAsync", ex);
        }
    }
}
