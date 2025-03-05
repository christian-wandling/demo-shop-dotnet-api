#region

using DemoShop.Domain.User.DTOs;
using DemoShop.Domain.User.Entities;
using DemoShop.Infrastructure.Features.Users;
using DemoShop.Infrastructure.Tests.Common.Base;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.Users.Repository;

[Trait("Feature", "User")]
public class GetUserByKeycloakIdAsyncTests : RepositoryTest
{
    private readonly UserRepository _sut;

    public GetUserByKeycloakIdAsyncTests()
    {
        var logger = Mock<ILogger>();
        _sut = new UserRepository(Context, logger);
    }

    [Fact]
    public async Task ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = Create<UserEntity>();
        await AddTestDataAsync(user);

        // Act
        var result = await _sut.GetUserByKeycloakIdAsync(user.KeycloakUserId.Value, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task ShouldReturnUserWithPhone_WhenUserExists()
    {
        // Arrange
        var user = Create<UserEntity>();
        user.UpdatePhone("+12345678");
        await AddTestDataAsync(user);

        // Act
        var result = await _sut.GetUserByKeycloakIdAsync(user.KeycloakUserId.Value, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
    }


    [Fact]
    public async Task ShouldReturnUserWithAddress_WhenUserExists()
    {
        // Arrange
        var unsavedUser = Create<UserEntity>();
        var savedUser = await AddTestDataAsync(unsavedUser);
        var address = Create<CreateAddressDto>() with { UserId = savedUser.Id };
        savedUser.SetInitialAddress(address);

        // Act
        var result = await _sut.GetUserByKeycloakIdAsync(savedUser.KeycloakUserId.Value, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(savedUser.Id);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenUserNotExists()
    {
        // Act
        var result = await _sut.GetUserByKeycloakIdAsync("noKeycloakId", CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnNull_WhenKeycloakIdDoesNotMatch()
    {
        // Arrange
        var user = Create<UserEntity>();
        await AddTestDataAsync(user);
        const string wrongKeycloakId = "wrongKeycloakId";

        // Act
        var result = await _sut.GetUserByKeycloakIdAsync(wrongKeycloakId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ShouldThrow_WhenKeycloakIdIsInvalid(string invalidKeycloakId)
    {
        // Act
        var act = () => _sut.GetUserByKeycloakIdAsync(invalidKeycloakId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
