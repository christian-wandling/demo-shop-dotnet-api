using System.Security.Claims;
using Ardalis.Result;
using DemoShop.Application.Features.Common.Constants;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.Common.Logging;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Common.Models;

public sealed record UserIdentity : IUserIdentity
{
    private UserIdentity(string email, Guid keycloakId, string firstName, string lastName)
    {
        Email = email;
        KeycloakId = keycloakId;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Email { get; }
    public Guid KeycloakId { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public static Result<IUserIdentity> FromClaimsPrincipal(ClaimsPrincipal? principal, ILogger<UserIdentity> logger)
    {
        if (principal?.Identity?.IsAuthenticated != true)
        {
            logger.LogIdentityUnauthenticated();
            return Result<IUserIdentity>.Unauthorized();
        }

        if (!ClaimRequirements.HasRequiredClaims(principal, out var claimValues))
        {
            logger.LogIdentityClaimsInvalid($"Missing required claims");
            return Result<IUserIdentity>.Unauthorized("Missing required claims");
        }

        if (!Guid.TryParse(claimValues[KeycloakClaimTypes.KeycloakId], out var parsedKeycloakId))
        {
            logger.LogIdentityClaimsInvalid("Invalid KeycloakId format");
            return Result<IUserIdentity>.Unauthorized("Invalid KeycloakId format");
        }

        var identity = new UserIdentity(
            claimValues[KeycloakClaimTypes.Email],
            parsedKeycloakId,
            claimValues[KeycloakClaimTypes.GivenName],
            claimValues[KeycloakClaimTypes.FamilyName]);

        logger.LogIdentityCreated(identity.Email);
        return Result<IUserIdentity>.Success(identity);
    }
}
