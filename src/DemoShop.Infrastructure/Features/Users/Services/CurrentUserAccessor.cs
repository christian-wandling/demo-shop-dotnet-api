#region

using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Features.Users.Services;

public sealed class CurrentUserAccessor(
    IUserRepository repository,
    IUserIdentityAccessor userIdentity,
    ILogger logger,
    ICacheService cacheService
) : ICurrentUserAccessor
{
    public async Task<Result<int>> GetId(CancellationToken cancellationToken)
    {
        var identityResult = userIdentity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
            return identityResult.Map();
        LogStarted(logger, identityResult.Value.KeycloakUserId);

        var cacheKey = cacheService.GenerateCacheKey("current-user-accessor", identityResult.Value.KeycloakUserId);
        var entity = cacheService.GetFromCache<UserEntity>(cacheKey)
                     ?? await GetFromDatabase(identityResult.Value.KeycloakUserId, cacheKey, cancellationToken);

        if (entity is null)
        {
            LogNotFound(logger, identityResult.Value.KeycloakUserId);
            return Result.NotFound("No user found");
        }

        LogSuccess(logger, entity.Id, entity.KeycloakUserId.Value);
        return Result.Success(entity.Id);
    }

    private async Task<UserEntity?> GetFromDatabase(
        string keycloakUserId, string cacheKey, CancellationToken cancellationToken)
    {
        var entity = await repository.GetUserByKeycloakIdAsync(keycloakUserId, cancellationToken);

        if (entity is null)
            return null;

        cacheService.SetCache(cacheKey, $"{entity.Id}");

        return entity;
    }

    private static void LogStarted(ILogger logger, string keycloakUserId) =>
        logger
            .ForContext("EventId", LoggerEventId.CurrentUserAccessorStarted)
            .Information("Attempting to get user for KeycloakUserId {KeycloakUserId}", keycloakUserId);

    private static void LogSuccess(ILogger logger, int userId, string keycloakUserId) =>
        logger
            .ForContext("EventId", LoggerEventId.CurrentUserAccessorSuccess)
            .Information("User with id {Id} found for KeycloakUserId {KeycloakUserId}",  userId, keycloakUserId);

    private static void LogNotFound(ILogger logger, string keycloakUserId) =>
        logger
            .ForContext("EventId", LoggerEventId.CurrentUserAccessorNotFound)
            .Error("No user found for KeycloakUserId {KeycloakUserId}", keycloakUserId);
}
