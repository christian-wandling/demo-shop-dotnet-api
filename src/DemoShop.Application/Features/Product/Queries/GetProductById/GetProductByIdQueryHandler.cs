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

namespace DemoShop.Application.Features.Product.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger logger,
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
            LogQueryStarted(logger, request.Id);

            var cacheKey = cacheService.GenerateCacheKey("product", request);
            var response = cacheService.GetFromCache<ProductResponse>(cacheKey)
                           ?? await GetFromDatabase(request.Id, cacheKey, cancellationToken);

            if (response is null)
            {
                LogNotFound(logger, request.Id);
                return Result.NotFound($"Product with Id {request.Id} not found");
            }

            LogQuerySuccess(logger, response.Id);
            return Result.Success(response);
        }
        catch (InvalidOperationException ex)
        {
            LogInvalidOperationException(logger, request.Id, ex.Message, ex);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            LogDatabaseException(logger, request.Id, ex.Message, ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<ProductResponse?> GetFromDatabase(
        int id, string cacheKey, CancellationToken cancellationToken
    )
    {
        var product = await repository.GetProductByIdAsync(id, cancellationToken);

        if (product is null)
            return null;

        var response = mapper.Map<ProductResponse>(product);
        cacheService.SetCache(cacheKey, response);

        return response;
    }

    private static void LogQueryStarted(ILogger logger, int productId) =>
        logger.ForContext("EventId", LoggerEventIds.GetProductByIdQueryStarted)
            .Information("Starting query to retrieve product with ID {ProductId}", productId);

    private static void LogQuerySuccess(ILogger logger, int productId) =>
        logger.ForContext("EventId", LoggerEventIds.GetProductByIdQuerySuccess)
            .Information("Successfully retrieved product with ID {ProductId}", productId);

    private static void LogNotFound(ILogger logger, int productId) =>
        logger.ForContext("EventId", LoggerEventIds.GetProductByIdQueryNotFound)
            .Information("Product with ID {ProductId} was not found", productId);

    private static void LogDatabaseException(ILogger logger, int productId, string errorMessage, Exception ex) =>
        logger.ForContext("EventId", LoggerEventIds.GetProductByIdDatabaseException)
            .Error(ex, "Database error occurred while retrieving product with ID {ProductId}. Error: {ErrorMessage}",
                productId, errorMessage);

    private static void
        LogInvalidOperationException(ILogger logger, int productId, string errorMessage, Exception ex) =>
        logger.ForContext("EventId", LoggerEventIds.GetProductByIdDomainException)
            .Error(ex, "Invalid operation while retrieving product with ID {ProductId}. Error: {ErrorMessage}",
                productId, errorMessage);
}
