#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.Product.Queries.GetAllProducts;

public sealed class GetAllProductsQueryHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetAllProductsQueryHandler> logger,
    ICacheService cacheService
)
    : IRequestHandler<GetAllProductsQuery, Result<ProductListResponse>>
{
    public async Task<Result<ProductListResponse>> Handle(GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        try
        {
            var cacheKey = cacheService.GenerateCacheKey("product", request);

            var cachedResponse = cacheService.GetFromCache<ProductListResponse>(cacheKey);

            if (cachedResponse is not null)
                return Result.Success(cachedResponse);

            var products = await repository.GetAllProductsAsync(cancellationToken);
            var response = mapper.Map<ProductListResponse>(products);
            cacheService.SetCache(cacheKey, response);

            return Result.Success(response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            logger.LogOperationFailed("Get all products", "", "", ex);
            return Result.Error(ex.Message);
        }
    }
}
