#region

using DemoShop.Domain.User.DTOs;
using DemoShop.Domain.User.Entities;
using DemoShop.Infrastructure.Features.Users;
using DemoShop.Infrastructure.Tests.Common.Base;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.Users.Repository;

[Trait("Feature", "User")]
public class UpdateUserAsyncTests : RepositoryTest
{
    private readonly UserRepository _sut;

    public UpdateUserAsyncTests(ITestOutputHelper output) : base(output)
    {
        _sut = new UserRepository(Context);
    }

    [Fact]
    public async Task ShouldUpdatePhone_WhenUserExists()
    {
        // Arrange
        var user = Create<UserEntity>();
        await AddTestDataAsync(user);
        const string newValue = "+98765432";
        user.UpdatePhone(newValue);

        // Act
        var result = await _sut.UpdateUserAsync(user, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Phone.Should().NotBeNull();
        result.Phone.Value.Should().Be(newValue);
    }

    [Fact]
    public async Task ShouldSetInitialAddress_WhenUserExists()
    {
        // Arrange
        var unsavedUser = Create<UserEntity>();
        var savedUser = await AddTestDataAsync(unsavedUser);
        var address = Create<CreateAddressDto>() with { UserId = savedUser.Id };
        savedUser.SetInitialAddress(address);

        // Act
        var result = await _sut.UpdateUserAsync(savedUser, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Address.Should().NotBeNull();
        result.Address.Should().BeEquivalentTo(address, options => options
            .ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldUpdateAddress_WhenUserExists()
    {
        // Arrange
        var unsavedUser = Create<UserEntity>();
        var savedUser = await AddTestDataAsync(unsavedUser);

        var address = Create<CreateAddressDto>() with { UserId = savedUser.Id };
        savedUser.SetInitialAddress(address);
        var userWithAddress = await _sut.UpdateUserAsync(savedUser, CancellationToken.None);

        var newAddress = Create<UpdateAddressDto>() with
        {
            UserId = userWithAddress.Id, AddressId = userWithAddress.Address!.Id
        };
        userWithAddress.UpdateAddress(newAddress);

        // Act
        var result = await _sut.UpdateUserAsync(userWithAddress, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Address.Should().NotBeNull();
        result.Address.Should().BeEquivalentTo(newAddress, options => options
            .ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldUpdateMultipleProperties_WhenUserExists()
    {
        // Arrange
        var unsavedUser = Create<UserEntity>();
        var savedUser = await AddTestDataAsync(unsavedUser);
        const string newPhone = "+98765432";
        var address = Create<CreateAddressDto>() with { UserId = savedUser.Id };
        savedUser.UpdatePhone(newPhone);
        savedUser.SetInitialAddress(address);

        // Act
        var result = await _sut.UpdateUserAsync(savedUser, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Phone.Should().NotBeNull();
        result.Phone.Value.Should().Be(newPhone);
        result.Address.Should().NotBeNull();
        result.Address.Should().BeEquivalentTo(address, options => options
            .ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldThrow_WhenUserEntityNull()
    {
        // Arrange
        UserEntity? nullUser = null;

        // Act
        var act = () => _sut.CreateUserAsync(nullUser!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
