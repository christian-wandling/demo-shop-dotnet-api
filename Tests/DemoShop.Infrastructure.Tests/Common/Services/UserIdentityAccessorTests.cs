#region

using System.Security.Claims;
using DemoShop.Application.Common.Models;
using DemoShop.Infrastructure.Common.Services;
using DemoShop.TestUtils.Common.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Infrastructure.Tests.Common.Services;

[Trait("Category", "Unit")]
[Trait("Layer", "Infrastructure")]
[Trait("Feature", "Common")]
public class UserIdentityAccessorTests : Test
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserIdentityAccessor _sut;

    public UserIdentityAccessorTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var logger = Substitute.For<ILogger<UserIdentity>>();
        _sut = new UserIdentityAccessor(_httpContextAccessor, logger);
    }

    [Fact]
    public void GetCurrentIdentity_WhenHttpContextIsNull_ShouldReturnFailedResult()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        // Act
        var result = _sut.GetCurrentIdentity();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void GetCurrentIdentity_WhenHttpContextUserIsNull_ShouldReturnFailedResult()
    {
        // Arrange
        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns((ClaimsPrincipal?)null);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var result = _sut.GetCurrentIdentity();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void GetCurrentIdentity_ShouldPassHttpContextUserToUserIdentity()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claimsPrincipal);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        _sut.GetCurrentIdentity();

        // Assert
        var context = _httpContextAccessor.Received(1).HttpContext;
        context.Should().NotBeNull();
        context.User = Arg.Any<ClaimsPrincipal>();
    }
}
