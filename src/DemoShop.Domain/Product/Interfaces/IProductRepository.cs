using DemoShop.Domain.Product.Entities;

namespace DemoShop.Domain.Product.Interfaces;

public interface IProductRepository
{
    Task<ProductEntity?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductEntity>> GetAllProductsAsync(CancellationToken cancellationToken = default);
}
