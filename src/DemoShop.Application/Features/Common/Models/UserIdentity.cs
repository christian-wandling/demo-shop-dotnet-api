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

        var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);
        logger.LogIdentityClaimsExtracted();

        return TryCreateFromClaims(claims, logger);
    }

    private static Result<IUserIdentity> TryCreateFromClaims(Dictionary<string, string> claims,
        ILogger<UserIdentity> logger)
    {
        if (!ValidateClaims(claims))
        {
            logger.LogIdentityClaimsInvalid("Missing required claims");
            return Result<IUserIdentity>.Unauthorized("Missing required claims");
        }

        var identity = new UserIdentity(
            claims[KeycloakClaimTypes.Email],
            Guid.Parse(claims[KeycloakClaimTypes.KeycloakId]),
            claims[KeycloakClaimTypes.GivenName],
            claims[KeycloakClaimTypes.FamilyName]);

        logger.LogIdentityCreated(identity.Email);
        return Result<IUserIdentity>.Success(identity);
    }

    private static bool ValidateClaims(Dictionary<string, string> claims) =>
        claims.ContainsKey(KeycloakClaimTypes.Email) &&
        claims.ContainsKey(KeycloakClaimTypes.KeycloakId) &&
        claims.ContainsKey(KeycloakClaimTypes.GivenName) &&
        claims.ContainsKey(KeycloakClaimTypes.FamilyName) &&
        Guid.TryParse(claims[KeycloakClaimTypes.KeycloakId], out _);
}
