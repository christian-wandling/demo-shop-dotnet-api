#region

using System.Security.Claims;
using Ardalis.Result;
using DemoShop.Application.Common.Constants;
using DemoShop.Application.Common.Models;
using DemoShop.Domain.Common.Logging;
using DemoShop.TestUtils.Common.Base;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Tests.Common.Models;

public class UserIdentityTests : Test
{
    private readonly ILogger<UserIdentity> _logger = Substitute.For<ILogger<UserIdentity>>();

    [Fact]
    public void FromClaimsPrincipal_WhenPrincipalIsNull_ReturnsUnauthorized()
    {
        // Act
        var result = UserIdentity.FromClaimsPrincipal(null, _logger);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
    }

    [Fact]
    public void FromClaimsPrincipal_WhenPrincipalIsNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        var result = UserIdentity.FromClaimsPrincipal(principal, _logger);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
    }

    [Fact]
    public void FromClaimsPrincipal_WhenRequiredClaimsAreMissing_ReturnsForbidden()
    {
        // Arrange
        var identity = new ClaimsIdentity(new List<Claim>(), "test");
        var principal = new ClaimsPrincipal(identity);
        principal.AddIdentity(new ClaimsIdentity([new Claim("some-claim", "value")], "test"));

        // Act
        var result = UserIdentity.FromClaimsPrincipal(principal, _logger);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Forbidden);
    }

    [Fact]
    public void FromClaimsPrincipal_WhenAllClaimsAreValid_ReturnsSuccess()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(KeycloakClaimTypes.Email, "test@example.com"),
            new(KeycloakClaimTypes.KeycloakUserId, "user123"),
            new(KeycloakClaimTypes.GivenName, "John"),
            new(KeycloakClaimTypes.FamilyName, "Doe")
        };
        var identity = new ClaimsIdentity(claims, "test", null, null);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserIdentity.FromClaimsPrincipal(principal, _logger);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be("test@example.com");
        result.Value.KeycloakUserId.Should().Be("user123");
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
    }

    [Fact]
    public void FromClaimsPrincipal_WhenRequiredClaimValueIsInvalid_ReturnsForbidden()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(KeycloakClaimTypes.Email, ""),
            new(KeycloakClaimTypes.KeycloakUserId, "user123"),
            new(KeycloakClaimTypes.GivenName, "John"),
            new(KeycloakClaimTypes.FamilyName, "Doe")
        };
        var identity = new ClaimsIdentity(claims, "test", null, null);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserIdentity.FromClaimsPrincipal(principal, _logger);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Forbidden);
    }

    [Fact]
    public void FromClaimsPrincipal_VerifyLoggerCalls()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        UserIdentity.FromClaimsPrincipal(principal, _logger);

        // Assert
        _logger.Received(1).LogAuthenticationFailed("Authentication failed: Principal is null or not authenticated");
    }
}
