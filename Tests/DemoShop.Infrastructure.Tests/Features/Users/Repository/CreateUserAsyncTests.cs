#region

using DemoShop.Domain.User.Entities;
using DemoShop.Infrastructure.Features.Users;
using DemoShop.Infrastructure.Tests.Common.Base;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.Users.Repository;

[Trait("Feature", "User")]
public class CreateUserAsyncTests : RepositoryTest
{
    private readonly UserRepository _sut;

    public CreateUserAsyncTests(ITestOutputHelper output) : base(output)
    {
        _sut = new UserRepository(Context);
    }

    [Fact]
    public async Task ShouldCreateUser_WhenUserEntityCorrect()
    {
        // Arrange
        var user = Create<UserEntity>();

        // Act
        var result = await _sut.CreateUserAsync(user, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task ShouldThrow_WhenUserIdAlreadyExists()
    {
        // Arrange
        var user = Create<UserEntity>();
        await AddTestDataAsync(user);

        // Act
        var act = () => _sut.CreateUserAsync(user, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ShouldThrow_WhenUserEntityNull()
    {
        // Act
        var act = () => _sut.CreateUserAsync(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ShouldPersistUserInDatabase_WhenCreated()
    {
        // Arrange
        var user = Create<UserEntity>();

        // Act
        await _sut.CreateUserAsync(user, CancellationToken.None);

        // Assert
        var savedUser = await Context.Set<UserEntity>()
            .FirstOrDefaultAsync(x => x.Id == user.Id);
        savedUser.Should().NotBeNull();
        savedUser.Should().BeEquivalentTo(user);
    }
}
