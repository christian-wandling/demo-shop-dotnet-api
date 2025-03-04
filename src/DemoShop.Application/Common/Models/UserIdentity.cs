#region

using System.Security.Claims;
using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Common.Constants;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using Serilog;

#endregion

namespace DemoShop.Application.Common.Models;

public sealed record UserIdentity : IUserIdentity
{
    private UserIdentity(string email, string keycloakUserId, string firstName, string lastName)
    {
        Email = Guard.Against.NullOrWhiteSpace(email, nameof(email));
        KeycloakUserId = Guard.Against.NullOrWhiteSpace(keycloakUserId, nameof(keycloakUserId));
        FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
        LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
    }

    public string Email { get; }
    public string KeycloakUserId { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public static Result<IUserIdentity> FromClaimsPrincipal(ClaimsPrincipal? principal, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        LogAuthenticationStarted(logger);

        if (principal?.Identity?.IsAuthenticated != true)
        {
            LogAuthenticationFailed(logger, "Authentication failed: Principal is null or not authenticated");
            return Result.Unauthorized("User is not authenticated");
        }

        if (!ClaimRequirements.HasRequiredClaims(principal, out var claimValues))
        {
            LogAuthenticationFailed(logger, "Missing required claims");
            return Result.Forbidden("Missing required claims");
        }

        try
        {
            var identity = new UserIdentity(
                claimValues[KeycloakClaimTypes.Email],
                claimValues[KeycloakClaimTypes.KeycloakUserId],
                claimValues[KeycloakClaimTypes.GivenName],
                claimValues[KeycloakClaimTypes.FamilyName]);

            LogAuthenticationSuccess(logger, identity.KeycloakUserId);
            return Result<IUserIdentity>.Success(identity);
        }
        catch (ArgumentException ex)
        {
            LogAuthenticationFailed(logger, ex.Message);
            return Result.Forbidden(ex.Message);
        }
    }

    private static void LogAuthenticationStarted(ILogger logger) =>
        logger.Debug("[{EventId}] Authentication started",
            LoggerEventIds.AuthenticationStarted);

    private static void LogAuthenticationSuccess(ILogger logger, string keycloakUserId) =>
        logger.Debug("[{EventId}] Authentication succeeded for user with KeycloakUserId {KeycloakUserId}",
            LoggerEventIds.AuthenticationSuccess, keycloakUserId);

    private static void LogAuthenticationFailed(ILogger logger, string error) =>
        logger.Error("[{EventId}] Authentication failed, Reason: {Error}",
            LoggerEventIds.AuthenticationFailed, error);
}
