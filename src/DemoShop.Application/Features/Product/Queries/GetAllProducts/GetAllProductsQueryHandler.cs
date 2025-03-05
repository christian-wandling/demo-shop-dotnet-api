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
            var response = cacheService.GetFromCache<ProductListResponse>(cacheKey)
                           ?? await GetFromDatabase(cacheKey, cancellationToken);

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

    private async Task<Result<ProductListResponse>> GetFromDatabase(
        string cacheKey,
        CancellationToken cancellationToken)
    {
        var products = await repository.GetAllProductsAsync(cancellationToken);
        var response = mapper.Map<ProductListResponse>(products);

        if (response.Items.Count > 0)
            cacheService.SetCache(cacheKey, response);

        return response;
    }

    private static void LogQueryStarted(ILogger logger) => logger
        .ForContext("EventId", LoggerEventId.GetAllProductsQueryStarted)
        .Debug("Starting query to retrieve all products");

    private static void LogQuerySuccess(ILogger logger, int productCount) => logger
            .ForContext("EventId", LoggerEventId.GetAllProductsQuerySuccess)
            .Information("Successfully retrieved {ProductCount} products", productCount);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) => logger
            .ForContext("EventId", LoggerEventId.GetAllProductsDatabaseException)
            .Error(ex, "Database error occurred while retrieving all products. Error: {ErrorMessage}",
            errorMessage);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.GetAllProductsDomainException)
        .Error(ex, "Invalid operation while retrieving all products. Error: {ErrorMessage}",
            errorMessage);
}
