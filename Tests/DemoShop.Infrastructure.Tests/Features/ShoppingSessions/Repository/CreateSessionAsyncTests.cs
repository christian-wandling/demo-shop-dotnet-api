#region

using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Infrastructure.Features.ShoppingSessions;
using DemoShop.Infrastructure.Tests.Common.Base;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.ShoppingSessions.Repository;

[Trait("Feature", "ShoppingSession")]
public class CreateSessionAsyncTests : RepositoryTest
{
    private readonly ShoppingSessionRepository _sut;

    public CreateSessionAsyncTests(ITestOutputHelper output) : base(output)
    {
        var logger = Mock<ILogger>();
        _sut = new ShoppingSessionRepository(Context, logger);
    }

    [Fact]
    public async Task ShouldCreateSession_WhenSessionEntityCorrect()
    {
        // Arrange
        var session = Create<ShoppingSessionEntity>();

        // Act
        var result = await _sut.CreateSessionAsync(session, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(session.Id);
    }

    [Fact]
    public async Task ShouldThrow_WhenShoppingSessionIdAlreadyExists()
    {
        // Arrange
        var session = Create<ShoppingSessionEntity>();
        await AddTestDataAsync(session);

        // Act
        var act = () => _sut.CreateSessionAsync(session, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ShouldThrow_WhenSessionEntityNull()
    {
        // Act
        var act = () => _sut.CreateSessionAsync(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ShouldPersistSessionInDatabase_WhenCreated()
    {
        // Arrange
        var session = Create<ShoppingSessionEntity>();

        // Act
        await _sut.CreateSessionAsync(session, CancellationToken.None);

        // Assert
        var savedSession = await Context.Set<ShoppingSessionEntity>()
            .FirstOrDefaultAsync(x => x.Id == session.Id);
        savedSession.Should().NotBeNull();
        savedSession.Should().BeEquivalentTo(session);
    }
}
