#region

using DemoShop.Application.Features.User.Commands.UpdateUserAddress;
using DemoShop.TestUtils.Common.Base;
using FluentValidation.TestHelper;

#endregion

namespace DemoShop.Application.Tests.Features.User.Commands;

[Trait("Feature", "User")]
public class UpdateUserAddressCommandValidatorTests : Test
{
    private readonly UpdateUserAddressCommandValidator _commandValidator = new();

    [Fact]
    public void Validate_WhenAddressIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenStreetIsNullOrEmpty_ShouldHaveValidationError(string street)
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.Street = street;

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.Street);
    }

    [Fact]
    public void Validate_WhenStreetExceeds50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.Street = new string('a', 51);

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.Street);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenApartmentIsNullOrEmpty_ShouldHaveValidationError(string apartment)
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.Apartment = apartment;

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.Apartment);
    }

    [Fact]
    public void Validate_WhenApartmentExceeds50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.Apartment = new string('a', 51);

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.Apartment);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenCityIsNullOrEmpty_ShouldHaveValidationError(string city)
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.City = city;

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.City);
    }

    [Fact]
    public void Validate_WhenCityExceeds50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.City = new string('a', 51);

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.City);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenZipIsNullOrEmpty_ShouldHaveValidationError(string zip)
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.Zip = zip;

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.Zip);
    }

    [Fact]
    public void Validate_WhenZipExceeds20Characters_ShouldHaveValidationError()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.Zip = new string('a', 21);

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.Zip);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenCountryIsNullOrEmpty_ShouldHaveValidationError(string country)
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.Country = country;

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.Country);
    }

    [Fact]
    public void Validate_WhenCountryExceeds50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.Country = new string('a', 51);

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.Country);
    }

    [Fact]
    public void Validate_WhenRegionExceeds50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        command.UpdateUserAddress.Region = new string('a', 51);

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUserAddress.Region);
    }
}
