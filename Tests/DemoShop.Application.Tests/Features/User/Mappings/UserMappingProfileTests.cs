#region

using AutoMapper;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Mappings;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Application.Tests.Features.User.Mappings;

public class UserMappingProfileTests : Test
{
    private readonly IMapper _mapper;

    public UserMappingProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<UserMappingProfile>());

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Configuration_IsValid()
    {
        // Arrange & Act & Assert
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<UserMappingProfile>());

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_UserEntityToUserResponse_MapsCorrectly()
    {
        // Arrange
        var phone = Create<Phone>();
        var address = Create<AddressEntity>();

        var userEntity = Create<UserEntity>();
        typeof(UserEntity)
            .GetProperty(nameof(UserEntity.Address))!
            .SetValue(userEntity, address);
        typeof(UserEntity)
            .GetProperty(nameof(UserEntity.Phone))!
            .SetValue(userEntity, phone);

        // Act
        var result = _mapper.Map<UserResponse>(userEntity);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(userEntity.Email.Value);
        result.Firstname.Should().Be(userEntity.PersonName.Firstname);
        result.Lastname.Should().Be(userEntity.PersonName.Lastname);
        result.Phone.Should().Be(phone.Value);
        result.Address.Should().NotBeNull();
    }

    [Fact]
    public void Map_UserEntityToUserPhoneResponse_MapsCorrectly()
    {
        // Arrange
        var phone = Create<Phone>();

        var userEntity = Create<UserEntity>();
        typeof(UserEntity)
            .GetProperty(nameof(UserEntity.Phone))!
            .SetValue(userEntity, phone);

        // Act
        var result = _mapper.Map<UserPhoneResponse>(userEntity);

        // Assert
        result.Should().NotBeNull();
        result.Phone.Should().Be(phone.Value);
    }

    [Fact]
    public void Map_AddressEntityToAddressResponse_MapsCorrectly()
    {
        // Arrange
        var addressEntity = Create<AddressEntity>();

        // Act
        var result = _mapper.Map<AddressResponse>(addressEntity);

        // Assert
        result.Should().NotBeNull();
        result.Street.Should().Be(addressEntity.Street);
        result.Apartment.Should().Be(addressEntity.Apartment);
        result.City.Should().Be(addressEntity.City);
        result.Zip.Should().Be(addressEntity.Zip);
        result.Region.Should().Be(addressEntity.Region);
        result.Country.Should().Be(addressEntity.Country);
    }

    [Fact]
    public void Map_UserEntityWithNullAddress_MapsOtherPropertiesCorrectly()
    {
        // Arrange
        var userEntity = Create<UserEntity>();

        // Act
        var result = _mapper.Map<UserResponse>(userEntity);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(userEntity.Email.Value);
        result.Firstname.Should().Be(userEntity.PersonName.Firstname);
        result.Lastname.Should().Be(userEntity.PersonName.Lastname);
        result.Phone.Should().Be(userEntity.Phone.Value);
        result.Address.Should().BeNull();
    }
}
