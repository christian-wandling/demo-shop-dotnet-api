#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
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
    ILogger<GetAllProductsQueryHandler> logger
)
    : IRequestHandler<GetAllProductsQuery, Result<ProductListResponse>>
{
    public async Task<Result<ProductListResponse>> Handle(GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        try
        {
            var result = await repository.GetAllProductsAsync(cancellationToken);

            return Result.Success(mapper.Map<ProductListResponse>(result));
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
