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
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Queries.GetAllProducts;

public sealed class GetAllProductsQueryHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger logger,
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
            LogQueryStarted(logger);

            var cacheKey = cacheService.GenerateCacheKey("product", request);

            var cachedResponse = cacheService.GetFromCache<ProductListResponse>(cacheKey);

            if (cachedResponse is not null)
                return Result.Success(cachedResponse);

            var products = await repository.GetAllProductsAsync(cancellationToken);
            var response = mapper.Map<ProductListResponse>(products);
            cacheService.SetCache(cacheKey, response);

            LogQuerySuccess(logger, response.Items.Count);

            return Result.Success(response);
        }
        catch (InvalidOperationException ex)
        {
            LogInvalidOperationException(logger, ex.Message, ex);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            LogDatabaseException(logger, ex.Message, ex);
            return Result.Error(ex.Message);
        }
    }

    private static void LogQueryStarted(ILogger logger) => logger.Information(
        "Starting query to retrieve all products {@EventId}", LoggerEventIds.GetAllProductsQueryStarted);

    private static void LogQuerySuccess(ILogger logger, int productCount) =>
        logger.Information("Successfully retrieved {ProductCount} products {@EventId}",
            productCount, LoggerEventIds.GetAllProductsQuerySuccess);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) =>
        logger.Error(ex, "Database error occurred while retrieving all products. Error: {ErrorMessage} {@EventId}",
            errorMessage, LoggerEventIds.GetAllProductsDatabaseException);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) =>
        logger.Error(ex, "Invalid operation while retrieving all products. Error: {ErrorMessage} {@EventId}",
            errorMessage, LoggerEventIds.GetAllProductsDomainException);
}
