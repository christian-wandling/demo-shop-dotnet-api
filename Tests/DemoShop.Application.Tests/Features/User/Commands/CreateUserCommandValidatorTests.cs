using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Features.User.Models;

namespace DemoShop.Application.Tests.Features.User.Commands;

public class CreateUserCommandValidatorTests : Test
{
    private readonly CreateUserCommandValidator _commandValidator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = Create<CreateUserCommand>();

        // Act
        var result = _commandValidator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenKeycloakUserIdIsNullOrEmpty_ShouldHaveValidationError(string keycloakUserId)
    {
        // Arrange
        var command = Create<CreateUserCommand>();
        ((TestUserIdentity)command.UserIdentity).KeycloakUserId = keycloakUserId;

        // Act
        var result = _commandValidator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "UserIdentity.KeycloakUserId" &&
            x.ErrorCode == "NotEmptyValidator");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    public void Validate_WhenEmailIsInvalid_ShouldHaveValidationError(string email)
    {
        // Arrange
        var command = Create<CreateUserCommand>();
        ((TestUserIdentity)command.UserIdentity).Email = email;

        // Act
        var result = _commandValidator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "UserIdentity.Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenFirstNameIsNullOrEmpty_ShouldHaveValidationError(string firstName)
    {
        // Arrange
        var command = Create<CreateUserCommand>();
        ((TestUserIdentity)command.UserIdentity).FirstName = firstName;

        // Act
        var result = _commandValidator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "UserIdentity.FirstName" &&
            x.ErrorCode == "NotEmptyValidator");
    }

    [Fact]
    public void Validate_WhenFirstNameExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var command = Create<CreateUserCommand>();
        ((TestUserIdentity)command.UserIdentity).FirstName = new string('a', 101);

        // Act
        var result = _commandValidator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "UserIdentity.FirstName" &&
            x.ErrorCode == "MaximumLengthValidator");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenLastNameIsNullOrEmpty_ShouldHaveValidationError(string lastName)
    {
        // Arrange
        var command = Create<CreateUserCommand>();
        ((TestUserIdentity)command.UserIdentity).LastName = lastName;

        // Act
        var result = _commandValidator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "UserIdentity.LastName" &&
            x.ErrorCode == "NotEmptyValidator");
    }

    [Fact]
    public void Validate_WhenLastNameExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var command = Create<CreateUserCommand>();
        ((TestUserIdentity)command.UserIdentity).LastName = new string('a', 101);

        // Act
        var result = _commandValidator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "UserIdentity.LastName" &&
            x.ErrorCode == "MaximumLengthValidator");
    }
}

