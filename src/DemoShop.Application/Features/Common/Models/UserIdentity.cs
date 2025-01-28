using System.Security.Claims;
using Ardalis.Result;
using DemoShop.Application.Features.Common.Constants;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Common.Models;

public sealed record UserIdentity : IUserIdentity
{
    private UserIdentity(string email, string keycloakId, string firstName, string lastName)
    {
        Email = email;
        KeycloakUserId = keycloakId;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Email { get; }
    public string KeycloakUserId { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public static Result<IUserIdentity> FromClaimsPrincipal(ClaimsPrincipal? principal, ILogger<UserIdentity> logger)
    {
        if (principal?.Identity?.IsAuthenticated != true)
        {
            logger.LogAuthFailed("Unauthenticated");
            return Result<IUserIdentity>.Unauthorized();
        }

        if (!ClaimRequirements.HasRequiredClaims(principal, out var claimValues))
        {
            logger.LogAuthFailed($"Missing required claims");
            return Result<IUserIdentity>.Unauthorized("Missing required claims");
        }

        var identity = new UserIdentity(
            claimValues[KeycloakClaimTypes.Email],
            claimValues[KeycloakClaimTypes.KeycloakUserId],
            claimValues[KeycloakClaimTypes.GivenName],
            claimValues[KeycloakClaimTypes.FamilyName]);

        return Result<IUserIdentity>.Success(identity);
    }
}
