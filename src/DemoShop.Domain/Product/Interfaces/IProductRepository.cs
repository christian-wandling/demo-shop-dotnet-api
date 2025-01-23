using Ardalis.Result;

namespace DemoShop.Domain.Product.Interfaces;

public interface IProductRepository
{
    Task<Result<IEnumerable<Entities.ProductEntity>>> GetAllProductsAsync();
    Task<Result<Entities.ProductEntity>> GetProductByIdAsync(int id);
}
