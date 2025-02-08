#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.User.DTOs;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Events;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Features.User.Models;

#endregion

namespace DemoShop.Domain.Tests.Features.User.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Feature", "User")]
public class UserEntityTests : Test
{
    public class Create : UserEntityTests
    {
        [Fact]
        public void Should_Create_User_Successfully()
        {
            // Arrange
            var userIdentity = Create<TestUserIdentity>();

            // Act
            var result = UserEntity.Create(userIdentity);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.KeycloakUserId.Value.Should().Be(userIdentity.KeycloakUserId);
            result.Value.Email.Value.Should().Be(userIdentity.Email);
            result.Value.PersonName.Firstname.Should().Be(userIdentity.FirstName);
            result.Value.PersonName.Lastname.Should().Be(userIdentity.LastName);
        }

        [Fact]
        public void Should_Add_UserCreatedDomainEvent()
        {
            // Arrange
            var userIdentity = Create<TestUserIdentity>();

            // Act
            var result = UserEntity.Create(userIdentity);

            // Assert
            result.Value.GetDomainEvents().Should().ContainSingle(e => e is UserCreatedDomainEvent);
        }

        [Fact]
        public void Should_Fail_When_UserIdentity_Is_Null()
        {
            // Act
            var action = () => UserEntity.Create(null!);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }
    }

    public class UpdatePhone : UserEntityTests
    {
        [Fact]
        public void Should_Update_Phone_Successfully()
        {
            // Arrange
            var user = UserEntity.Create(Create<TestUserIdentity>()).Value;
            var newPhone = "+1234567890";

            // Act
            var result = user.UpdatePhone(newPhone);

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.Phone!.Value.Should().Be(newPhone);
            user.GetDomainEvents().Should().ContainSingle(e => e is UserPhoneUpdatedDomainEvent);
        }

        [Fact]
        public void Should_Allow_Null_Phone()
        {
            // Arrange
            var user = UserEntity.Create(Create<TestUserIdentity>()).Value;

            // Act
            var result = user.UpdatePhone(null);

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.Phone.Value.Should().BeNull();
        }
    }

    public class SetInitialAddress : UserEntityTests
    {
        [Fact]
        public void Should_Set_Initial_Address_Successfully()
        {
            // Arrange
            var user = UserEntity.Create(Create<TestUserIdentity>()).Value;
            var createAddressDto = Create<CreateAddressDto>();

            // Act
            var result = user.SetInitialAddress(createAddressDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.Address.Should().NotBeNull();
            user.GetDomainEvents().Should().ContainSingle(e => e is UserAddressUpdatedDomainEvent);
        }

        [Fact]
        public void Should_Throw_When_Address_Already_Exists()
        {
            // Arrange
            var user = UserEntity.Create(Create<TestUserIdentity>()).Value;
            var createAddressDto = Create<CreateAddressDto>();
            user.SetInitialAddress(createAddressDto);

            // Act
            var action = () => user.SetInitialAddress(createAddressDto);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Address already set. Use UpdateAddress to modify the existing address.");
        }
    }

    public class UpdateAddress : UserEntityTests
    {
        [Fact]
        public void Should_Update_Address_Successfully()
        {
            // Arrange
            var user = UserEntity.Create(Create<TestUserIdentity>()).Value;
            var createAddressDto = Create<CreateAddressDto>();
            user.SetInitialAddress(createAddressDto);
            var updateAddressDto = Create<UpdateAddressDto>();

            // Act
            var result = user.UpdateAddress(updateAddressDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.GetDomainEvents().Should().Contain(e => e is UserAddressUpdatedDomainEvent);
        }

        [Fact]
        public void Should_Throw_When_No_Address_Exists()
        {
            // Arrange
            var user = UserEntity.Create(Create<TestUserIdentity>()).Value;
            var updateAddressDto = Create<UpdateAddressDto>();

            // Act
            var action = () => user.UpdateAddress(updateAddressDto);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Address not found. Use SetInitalAddress to create an address.");
        }
    }
}
