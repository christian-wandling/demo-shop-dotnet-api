#region

using System.Security.Claims;
using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Common.Constants;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Common.Models;

public sealed record UserIdentity : IUserIdentity
{
    private UserIdentity(string email, string keycloakId, string firstName, string lastName)
    {
        Email = Guard.Against.NullOrWhiteSpace(email, nameof(email));
        KeycloakUserId = Guard.Against.NullOrWhiteSpace(keycloakId, nameof(keycloakId));
        FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
        LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
    }

    public string Email { get; }
    public string KeycloakUserId { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public static Result<IUserIdentity> FromClaimsPrincipal(ClaimsPrincipal? principal, ILogger<UserIdentity> logger)
    {
        if (principal?.Identity?.IsAuthenticated != true)
        {
            logger.LogAuthFailed("Authentication failed: Principal is null or not authenticated");
            return Result.Unauthorized("User is not authenticated");
        }

        if (!ClaimRequirements.HasRequiredClaims(principal, out var claimValues))
        {
            logger.LogAuthFailed("Missing required claims");
            return Result.Forbidden("Missing required claims");
        }

        try
        {
            var identity = new UserIdentity(
                claimValues[KeycloakClaimTypes.Email],
                claimValues[KeycloakClaimTypes.KeycloakUserId],
                claimValues[KeycloakClaimTypes.GivenName],
                claimValues[KeycloakClaimTypes.FamilyName]);

            return Result<IUserIdentity>.Success(identity);
        }
        catch (ArgumentException ex)
        {
            return Result.Unauthorized(ex.Message);
        }
    }
}
