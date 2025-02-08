using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;
using DemoShop.TestUtils.Common.Base;
using FluentValidation.TestHelper;

namespace DemoShop.Application.Tests.Features.ShoppingSession.Commands;

public class UpdateCartItemQuantityCommandValidatorTests : Test
{
    private readonly UpdateCartItemQuantityCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsGreaterThanZero_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateCartItemQuantityCommand(
            Create<int>(),
            new UpdateCartItemQuantityRequest()
            {
                Quantity = Create<int>(),
            }
        );
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenUserIdIsNotGreaterThanZero_ShouldHaveValidationError(int invalidUserId)
    {
        // Arrange
        var command = new UpdateCartItemQuantityCommand(
            invalidUserId,
            new UpdateCartItemQuantityRequest()
            {
                Quantity = Create<int>(),
            }
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("'Id' must be greater than '0'.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenQuantityIsNotGreaterThanZero_ShouldHaveValidationError(int invalidQuantity)
    {
        // Arrange
        var command = new UpdateCartItemQuantityCommand(
            Create<int>(),
            new UpdateCartItemQuantityRequest()
            {
                Quantity = invalidQuantity
            }
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateCartItem.Quantity)
            .WithErrorMessage("'Update Cart Item Quantity' must be greater than '0'.");
    }
}
