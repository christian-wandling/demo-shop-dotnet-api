using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.Interfaces;
using MediatR;

namespace DemoShop.Application.Features.Product.Queries.GetAllProducts;

public sealed class GetAllProductsQueryHandler(
    IMapper mapper,
    IProductRepository repository
)
    : IRequestHandler<GetAllProductsQuery, Result<ProductListResponse>>
{
    public async Task<Result<ProductListResponse>> Handle(GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetAllProductsAsync(cancellationToken).ConfigureAwait(false);

        return Result<ProductListResponse>.Success(mapper.Map<ProductListResponse>(result));
    }
}
