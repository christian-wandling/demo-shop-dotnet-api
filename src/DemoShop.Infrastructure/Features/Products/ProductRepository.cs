using Ardalis.GuardClauses;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.Interfaces;
using DemoShop.Infrastructure.Common;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DemoShop.Infrastructure.Features.Products;

public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public async Task<ProductEntity?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(id, nameof(id));

        return await context.Query<ProductEntity>()
            .Include(p => p.Categories)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<ProductEntity>> GetAllProductsAsync(CancellationToken cancellationToken) =>
        await context.Query<ProductEntity>()
            .Include(p => p.Categories)
            .Include(p => p.Images)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
