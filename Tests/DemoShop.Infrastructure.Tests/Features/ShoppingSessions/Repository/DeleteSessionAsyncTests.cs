#region

using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Infrastructure.Features.ShoppingSessions;
using DemoShop.Infrastructure.Tests.Common.Base;
using Serilog;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.ShoppingSessions.Repository;

[Trait("Feature", "ShoppingSession")]
public class DeleteSessionAsyncTests : RepositoryTest
{
    private readonly ShoppingSessionRepository _sut;

    public DeleteSessionAsyncTests(ITestOutputHelper output) : base(output)
    {
        var logger = Mock<ILogger>();
        _sut = new ShoppingSessionRepository(Context, logger);
    }

    [Fact]
    public async Task ShouldDeleteSession_WhenSessionEntityCorrect()
    {
        // Arrange
        var session = Create<ShoppingSessionEntity>();
        await AddTestDataAsync(session);

        // Act
        var result = await _sut.DeleteSessionAsync(session, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldThrow_WhenSessionEntityNull()
    {
        // Act
        var act = () => _sut.DeleteSessionAsync(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
