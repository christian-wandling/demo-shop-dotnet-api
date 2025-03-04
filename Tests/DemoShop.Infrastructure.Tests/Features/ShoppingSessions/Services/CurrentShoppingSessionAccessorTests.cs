#region

using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.Infrastructure.Features.ShoppingSessions.Services;
using DemoShop.TestUtils.Common.Base;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.ShoppingSessions.Services;

[Trait("Category", "Unit")]
[Trait("Layer", "Infrastructure")]
[Trait("Feature", "ShoppingSession")]
public class CurrentShoppingSessionAccessorTests : Test
{
    private readonly ICurrentUserAccessor _currentUser;
    private readonly IShoppingSessionRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly CurrentShoppingSessionAccessor _sut;

    public CurrentShoppingSessionAccessorTests()
    {
        _repository = Mock<IShoppingSessionRepository>();
        _currentUser = Mock<ICurrentUserAccessor>();
        var logger = Mock<ILogger>();
        _cacheService = Mock<ICacheService>();
        _sut = new CurrentShoppingSessionAccessor(_repository, _currentUser, logger, _cacheService);
    }

    [Fact]
    public async Task GetCurrent_WhenUserIdFails_ReturnsFailure()
    {
        // Arrange
        _currentUser.GetId(CancellationToken.None).Returns(Result.Error());

        // Act
        var result = await _sut.GetCurrent(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task GetCurrent_WhenSessionNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Create<int>();
        _currentUser.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _repository.GetSessionByUserIdAsync(userId, CancellationToken.None).Returns((ShoppingSessionEntity?)null);

        // Act
        var result = await _sut.GetCurrent(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task GetCurrent_WhenSessionExists_ReturnsSuccess()
    {
        // Arrange
        var userId = Create<int>();
        var session = Create<ShoppingSessionEntity>();
        _currentUser.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _repository.GetSessionByUserIdAsync(userId, CancellationToken.None).Returns(session);

        // Act
        var result = await _sut.GetCurrent(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(session);
    }
}
