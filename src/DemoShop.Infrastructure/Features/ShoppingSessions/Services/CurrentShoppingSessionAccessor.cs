#region

using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Features.ShoppingSessions.Services;

public sealed class CurrentShoppingSessionAccessor(
    IShoppingSessionRepository repository,
    ICurrentUserAccessor currentUser,
    ILogger logger,
    ICacheService cacheService
)
    : ICurrentShoppingSessionAccessor
{
    public async Task<Result<ShoppingSessionEntity>> GetCurrent(CancellationToken cancellationToken)
    {
        var userIdResult = await currentUser.GetId(cancellationToken);

        if (!userIdResult.IsSuccess)
            return userIdResult.Map();

        LogStarted(logger, userIdResult.Value);

        var cacheKey = cacheService.GenerateCacheKey("current-session-accessor", userIdResult.Value);
        var entity = cacheService.GetFromCache<ShoppingSessionEntity>(cacheKey)
                     ?? await GetFromDatabase(userIdResult.Value, cacheKey, cancellationToken);

        if (entity is null)
        {
            LogNotFound(logger, userIdResult.Value);
            return Result.NotFound("No active shopping session session found for current user");
        }

        LogSuccess(logger, entity);
        return Result.Success(entity);
    }

    private async Task<ShoppingSessionEntity?> GetFromDatabase(
        int userId, string cacheKey, CancellationToken cancellationToken)
    {
        var entity = await repository.GetSessionByUserIdAsync(userId, cancellationToken);

        if (entity is null)
            return null;

        cacheService.SetCache(cacheKey, $"{entity.Id}");

        return entity;
    }

    private static void LogStarted(ILogger logger, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.CurrentShoppingSessionAccessorStarted)
            .Information("Attempting to get current shopping session for user {UserId}", userId);

    private static void LogSuccess(ILogger logger, ShoppingSessionEntity session) =>
        logger
            .ForContext("EventId", LoggerEventId.CurrentShoppingSessionAccessorSuccess)
            .Information("Shopping session with id {Id} found for user {UserId}", session.Id, session.UserId);

    private static void LogNotFound(ILogger logger, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.CurrentShoppingSessionAccessorNotFound)
            .Error("No shopping session found for user {UserId}", userId);
}
