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

namespace DemoShop.Application.Features.Product.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetProductByIdQueryHandler> logger,
    ICacheService cacheService
)
    : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NegativeOrZero(request.Id, nameof(request.Id));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        try
        {
            var cacheKey = cacheService.GenerateCacheKey("product", request);

            var cachedResponse = cacheService.GetFromCache<ProductResponse>(cacheKey);

            if (cachedResponse is not null)
                return Result.Success(cachedResponse);

            var product = await repository.GetProductByIdAsync(request.Id, cancellationToken);

            if (product is not null)
            {
                var response = mapper.Map<ProductResponse>(product);
                cacheService.SetCache(cacheKey, response);

                return Result.Success(response);
            }

            logger.LogOperationFailed("Get Product By Id", "Id", $"{request.Id}", null);
            return Result.NotFound($"Product with Id {request.Id} not found");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            logger.LogOperationFailed("Get product by Id", "Id", $"{request.Id}", ex);
            return Result.Error(ex.Message);
        }
    }
}
