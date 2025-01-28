using Ardalis.Result;
using DemoShop.Domain.Product.Entities;

namespace DemoShop.Domain.Product.Interfaces;

public interface IProductRepository
{
    Task<Result<IEnumerable<ProductEntity>>> GetAllProductsAsync();
    Task<Result<ProductEntity>> GetProductByIdAsync(int id);
}
