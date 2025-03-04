#region

using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using DemoShop.Infrastructure.Features.Users.Services;
using DemoShop.TestUtils.Common.Base;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.Users.Services;

[Trait("Category", "Unit")]
[Trait("Layer", "Infrastructure")]
[Trait("Feature", "User")]
public class CurrentUserAccessorTests : Test
{
    private readonly IUserRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly CurrentUserAccessor _sut;
    private readonly IUserIdentityAccessor _userIdentity;

    public CurrentUserAccessorTests()
    {
        _repository = Mock<IUserRepository>();
        _userIdentity = Mock<IUserIdentityAccessor>();
        var logger = Mock<ILogger>();
        _cacheService = Mock<ICacheService>();
        _sut = new CurrentUserAccessor(_repository, _userIdentity, logger, _cacheService);
    }

    [Fact]
    public async Task GetId_WhenIdentityResultFails_ReturnsFailedResult()
    {
        // Arrange
        var expectedError = Result.Error();
        _userIdentity.GetCurrentIdentity().Returns(expectedError);

        // Act
        var result = await _sut.GetId(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(expectedError.Status);
        await _repository.DidNotReceive().GetUserByKeycloakIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetId_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var identity = Create<IUserIdentity>();
        _userIdentity.GetCurrentIdentity().Returns(Result.Success(identity));
        _repository.GetUserByKeycloakIdAsync(identity.KeycloakUserId, Arg.Any<CancellationToken>())
            .Returns((UserEntity?)null);

        // Act
        var result = await _sut.GetId(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task GetId_WhenUserFound_ReturnsUserId()
    {
        // Arrange
        var identity = Create<IUserIdentity>();
        var user = Create<UserEntity>();

        _userIdentity.GetCurrentIdentity().Returns(Result.Success(identity));
        _repository.GetUserByKeycloakIdAsync(identity.KeycloakUserId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.GetId(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(user.Id);
        await _repository.Received(1).GetUserByKeycloakIdAsync(identity.KeycloakUserId, Arg.Any<CancellationToken>());
    }
}
