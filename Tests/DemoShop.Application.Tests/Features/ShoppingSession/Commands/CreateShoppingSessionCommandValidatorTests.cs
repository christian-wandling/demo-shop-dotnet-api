#region

using DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;
using DemoShop.TestUtils.Common.Base;
using FluentValidation.TestHelper;

#endregion

namespace DemoShop.Application.Tests.Features.ShoppingSession.Commands;

public class CreateShoppingSessionCommandValidatorTests : Test
{
    private readonly CreateShoppingSessionCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenUserIdIsGreaterThanZero_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateShoppingSessionCommand(
            Create<int>()
        );
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenUserIdIsNotGreaterThanZero_ShouldHaveValidationError(int invalidUserId)
    {
        // Arrange
        var command = new CreateShoppingSessionCommand(
            invalidUserId
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("'User Id' must be greater than '0'.");
    }
}
