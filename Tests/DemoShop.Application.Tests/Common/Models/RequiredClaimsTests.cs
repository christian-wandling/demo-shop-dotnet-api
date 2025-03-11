#region

using System.Security.Claims;
using DemoShop.Application.Common.Constants;
using DemoShop.Application.Common.Models;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Application.Tests.Common.Models;

[Trait("Feature", "Common")]
public class ClaimRequirementsTests : Test
{
    [Fact]
    public void HasRequiredClaims_WhenAllClaimsPresent_ReturnsTrue()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(KeycloakClaimTypes.Email, "test@example.com"),
            new(KeycloakClaimTypes.KeycloakUserId, "user123"),
            new(KeycloakClaimTypes.GivenName, "John"),
            new(KeycloakClaimTypes.FamilyName, "Doe")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimRequirements.HasRequiredClaims(principal, out var claimValues);

        // Assert
        result.Should().BeTrue();
        claimValues.Should().HaveCount(4);
        claimValues[KeycloakClaimTypes.Email].Should().Be("test@example.com");
        claimValues[KeycloakClaimTypes.KeycloakUserId].Should().Be("user123");
        claimValues[KeycloakClaimTypes.GivenName].Should().Be("John");
        claimValues[KeycloakClaimTypes.FamilyName].Should().Be("Doe");
    }

    [Fact]
    public void HasRequiredClaims_WhenMissingClaim_ReturnsFalse()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(KeycloakClaimTypes.Email, "test@example.com"),
            new(KeycloakClaimTypes.KeycloakUserId, "user123"),
            new(KeycloakClaimTypes.GivenName, "John")
            // Missing FamilyName claim
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimRequirements.HasRequiredClaims(principal, out var claimValues);

        // Assert
        result.Should().BeFalse();
        claimValues.Count.Should().Be(3);
    }

    [Fact]
    public void HasRequiredClaims_WhenEmptyClaimValue_ReturnsFalse()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(KeycloakClaimTypes.Email, "test@example.com"),
            new(KeycloakClaimTypes.KeycloakUserId, "user123"),
            new(KeycloakClaimTypes.GivenName, "John"),
            new(KeycloakClaimTypes.FamilyName, "") // Empty claim value
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimRequirements.HasRequiredClaims(principal, out var claimValues);

        // Assert
        result.Should().BeFalse();
        claimValues.Count.Should().Be(3);
    }

    [Fact]
    public void HasRequiredClaims_WhenNullPrincipal_ThrowsArgumentNullException()
    {
        // Arrange
        ClaimsPrincipal principal = null!;

        // Act & Assert
        var act = () => ClaimRequirements.HasRequiredClaims(principal, out _);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("principal");
    }
}
