#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Interfaces;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;

public sealed class GetShoppingSessionByUserIdQueryHandler(
    IMapper mapper,
    IShoppingSessionRepository repository,
    ILogger logger,
    ICacheService cacheService
)
    : IRequestHandler<GetShoppingSessionByUserIdQuery, Result<ShoppingSessionResponse>>
{
    public async Task<Result<ShoppingSessionResponse>> Handle(GetShoppingSessionByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        try
        {
            LogQueryStarted(logger, request.UserId);

            var cacheKey = cacheService.GenerateCacheKey("shoppingSession", request);
            var response = cacheService.GetFromCache<ShoppingSessionResponse>(cacheKey)
                           ?? await GetFromDatabase(request.UserId, cacheKey, cancellationToken);

            if (response is null)
            {
                LogNotFound(logger, request.UserId);
                return Result.NotFound("Shopping session not found");
            }

            LogQuerySuccess(logger, response.Id);
            return Result.Success(response);
        }
        catch (InvalidOperationException ex)
        {
            LogInvalidOperationException(logger, request.UserId, ex.Message, ex);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            LogDatabaseException(logger, request.UserId, ex.Message, ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<ShoppingSessionResponse?> GetFromDatabase(
        int userId, string cacheKey, CancellationToken cancellationToken)
    {
        var user = await repository.GetSessionByUserIdAsync(userId, cancellationToken);

        if (user is null)
            return null;

        var response = mapper.Map<ShoppingSessionResponse>(user);
        cacheService.SetCache(cacheKey, response);

        return response;
    }

    private static void LogQueryStarted(ILogger logger, int userId) =>
        logger.ForContext("EventId", LoggerEventIds.GetShoppingSessionByUserIdQueryStarted)
            .Information("Starting query to retrieve shopping session with UserId {UserId}", userId);

    private static void LogQuerySuccess(ILogger logger, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.GetShoppingSessionByUserIdQuerySuccess)
            .Information("Successfully retrieved shopping session with ID {SessionId}", sessionId);

    private static void LogNotFound(ILogger logger, int userId) =>
        logger.ForContext("EventId", LoggerEventIds.GetShoppingSessionByUserIdQueryNotFound)
            .Information("ShoppingSession with UserId {UserId} was not found", userId);

    private static void
        LogDatabaseException(ILogger logger, int userId, string errorMessage, Exception ex) =>
        logger.ForContext("EventId", LoggerEventIds.GetShoppingSessionByUserIdDatabaseException)
            .Error(ex,
                "Database error occurred while retrieving shopping session with ID {UserId}. Error: {ErrorMessage}",
                userId, errorMessage);

    private static void
        LogInvalidOperationException(ILogger logger, int userId, string errorMessage, Exception ex) =>
        logger.ForContext("EventId", LoggerEventIds.GetShoppingSessionByUserIdDomainException)
            .Error(ex, "Invalid operation while retrieving shopping session with ID {UserId}. Error: {ErrorMessage}",
                userId, errorMessage);
}
