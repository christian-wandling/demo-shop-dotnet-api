using Ardalis.Result;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.Interfaces;
using MediatR;

namespace DemoShop.Application.Features.Product.Queries.GetAllProducts;

public sealed class GetAllProductsQueryHandler(
    IProductRepository repository
)
    : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductEntity>>>
{
    public async Task<Result<IEnumerable<ProductEntity>>> Handle(GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await repository.GetAllProductsAsync(cancellationToken).ConfigureAwait(false);

        return Result<IEnumerable<ProductEntity>>.Success(products);
    }
}
