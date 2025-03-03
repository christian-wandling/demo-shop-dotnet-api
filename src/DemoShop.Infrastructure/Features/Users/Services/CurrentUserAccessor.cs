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
    ILogger logger
) : ICurrentUserAccessor
{
    public async Task<Result<int>> GetId(CancellationToken cancellationToken)
    {
        var identityResult = userIdentity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
            return identityResult.Map();

        var user = await repository.GetUserByKeycloakIdAsync(identityResult.Value.KeycloakUserId, cancellationToken);

        if (user is null)
        {
            LogNotFound(logger, identityResult.Value.KeycloakUserId);
            return Result.NotFound("No user found");
        }

        LogSuccess(logger, user);
        return Result.Success(user.Id);
    }

    private static void LogSuccess(ILogger logger, UserEntity user) =>
        logger.Information(
            "[{EventId}] User with id {Id} found for KeycloakUserId {KeycloakUserId}",
            LoggerEventIds.CurrentUserAccessorSuccess,
            user.Id, user.KeycloakUserId);

    private static void LogNotFound(ILogger logger, string keycloakUserId) =>
        logger.Error(
            "[{EventId}] No user found for KeycloakUserId {KeycloakUserId}",
            LoggerEventIds.CurrentUserAccessorNotFound,
            keycloakUserId);
}
