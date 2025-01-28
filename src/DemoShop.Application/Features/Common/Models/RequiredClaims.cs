using System.Security.Claims;
using Ardalis.GuardClauses;
using DemoShop.Application.Features.Common.Constants;

namespace DemoShop.Application.Features.Common.Models;

public static class ClaimRequirements
{
    private static readonly IEnumerable<string> RequiredClaims =
    [
        KeycloakClaimTypes.Email,
        KeycloakClaimTypes.KeycloakUserId,
        KeycloakClaimTypes.GivenName,
        KeycloakClaimTypes.FamilyName
    ];

    public static bool HasRequiredClaims(ClaimsPrincipal principal, out IDictionary<string, string> claimValues)
    {
        Guard.Against.Null(principal, nameof(principal));
        claimValues = new Dictionary<string, string>();

        foreach (var claimType in RequiredClaims)
        {
            var claim = principal.FindFirst(claimType)?.Value;
            if (string.IsNullOrEmpty(claim))
                return false;

            claimValues[claimType] = claim;
        }

        return true;
    }
}
