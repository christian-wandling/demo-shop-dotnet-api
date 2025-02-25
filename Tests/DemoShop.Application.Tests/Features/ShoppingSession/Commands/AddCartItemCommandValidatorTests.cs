#region

using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Application.Tests.Features.ShoppingSession.Commands;

public class AddCartItemCommandValidatorTests : Test
{
    private readonly AddCartItemCommandValidator _sut = new();

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Validate_WhenProductIdIsGreaterThanZero_ShouldBeValid(int productId)
    {
        // Arrange
        var command = new AddCartItemCommand(
            new AddCartItemRequest { ProductId = productId }
        );

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Validate_WhenProductIdIsZeroOrLess_ShouldBeInvalid(int productId)
    {
        // Arrange
        var command = new AddCartItemCommand(
            new AddCartItemRequest { ProductId = productId }
        );

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("AddCartItem.ProductId");
        result.Errors[0].ErrorMessage.Should().Be("'Add Cart Item Product Id' must be greater than '0'.");
    }
}
