using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.User.Commands.UpdateUserPhone;
using DemoShop.TestUtils.Common.Base;
using FluentValidation.TestHelper;

namespace DemoShop.Application.Tests.Features.User.Commands;

public class UpdateUserPhoneCommandValidatorTests : Test
{
    private readonly UpdateUserPhoneCommandValidator _commandValidator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateUserPhoneCommand(
            new UpdateUserPhoneRequest { Phone = "+1234567890" }
        );

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUpdateUserPhoneIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateUserPhoneCommand(
            new UpdateUserPhoneRequest { Phone = null }
        );

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUser.Phone);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenPhoneIsNullOrEmpty_ShouldHaveValidationError(string phone)
    {
        // Arrange
        var command = new UpdateUserPhoneCommand(
            new UpdateUserPhoneRequest { Phone = phone }
        );

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUser.Phone)
            .WithErrorMessage("'Update User Phone' must not be empty.");
    }

    [Theory]
    [InlineData("abc123")]
    [InlineData("++1234567890")]
    [InlineData("12345678901234567890")]
    public void Validate_WhenPhoneFormatIsInvalid_ShouldHaveValidationError(string phone)
    {
        // Arrange
        var command = new UpdateUserPhoneCommand(
            new UpdateUserPhoneRequest { Phone = phone }
        );

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateUser.Phone)
            .WithErrorMessage("Invalid phone number");
    }

    [Theory]
    [InlineData("+1234567890")]
    [InlineData("1234567890")]
    [InlineData("+44 1234567890")]
    public void Validate_WhenPhoneFormatIsValid_ShouldNotHaveValidationError(string phone)
    {
        var command = new UpdateUserPhoneCommand(
            new UpdateUserPhoneRequest() { Phone = phone }
        );

        // Act
        var result = _commandValidator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UpdateUser.Phone);
    }
}
