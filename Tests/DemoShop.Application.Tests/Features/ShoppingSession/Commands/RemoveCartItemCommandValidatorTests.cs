using DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;
using DemoShop.TestUtils.Common.Base;
using FluentValidation.TestHelper;

namespace DemoShop.Application.Tests.Features.ShoppingSession.Commands;

public class RemoveCartItemCommandValidatorTests : Test
{
    private readonly RemoveCartItemCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsGreaterThanZero_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new RemoveCartItemCommand(
            Create<int>()
        );
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenIdIsNotGreaterThanZero_ShouldHaveValidationError(int invalidUserId)
    {
        // Arrange
        var command = new RemoveCartItemCommand(
            invalidUserId
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("'Id' must be greater than '0'.");
    }
}
