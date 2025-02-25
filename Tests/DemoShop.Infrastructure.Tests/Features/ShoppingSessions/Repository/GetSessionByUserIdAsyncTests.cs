#region

using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Infrastructure.Features.ShoppingSessions;
using DemoShop.Infrastructure.Tests.Common.Base;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.ShoppingSessions.Repository;

[Trait("Feature", "ShoppingSession")]
public class GetSessionByUserIdAsyncTests : RepositoryTest
{
    private readonly ShoppingSessionRepository _sut;

    public GetSessionByUserIdAsyncTests(ITestOutputHelper output) : base(output)
    {
        _sut = new ShoppingSessionRepository(Context);
    }

    [Fact]
    public async Task ShouldReturnShoppingSession_WhenUserIdCorrect()
    {
        // Arrange
        const int userId = 1;
        var session = Enumerable.Range(0, 3)
            .Select(_ => Create<ShoppingSessionEntity>())
            .Select(_ => ShoppingSessionEntity.Create(
                userId
            ).Value)
            .ToList();
        await AddTestDataRangeAsync(session);

        // Act
        var result = await _sut.GetSessionByUserIdAsync(userId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenNoSessionForUserFound()
    {
        // Act
        var result = await _sut.GetSessionByUserIdAsync(999, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ShouldThrow_WhenUserIdIsInvalid(int invalidUserId)
    {
        // Act
        var act = () => _sut.GetSessionByUserIdAsync(invalidUserId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
