#region

using Ardalis.Result;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.ShoppingSession;

[Trait("Feature", "ShoppingSession")]
public class CartItemEntityTests : Test
{
    public class Create : CartItemEntityTests
    {
        [Fact]
        public void Create_Successfully()
        {
            // Arrange
            var shoppingSessionId = Create<int>(); // Ensure positive
            var productId = Create<int>();

            // Act
            var result = CartItemEntity.Create(shoppingSessionId, productId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.ShoppingSessionId.Should().Be(shoppingSessionId);
            result.Value.ProductId.Should().Be(productId);
            result.Value.Quantity.Value.Should().Be(1);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Fail_WithInvalidShoppingSessionId(int invalidId)
        {
            // Arrange
            var productId = Create<int>();

            // Act
            var action = () => CartItemEntity.Create(invalidId, productId);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Fail_WithInvalidProductId(int invalidId)
        {
            // Arrange
            var shoppingSessionId = Create<int>();

            // Act
            var action = () => CartItemEntity.Create(shoppingSessionId, invalidId);

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }

    public class UpdateQuantity : CartItemEntityTests
    {
        [Fact]
        public void SetNewQuantity()
        {
            // Arrange
            var cartItem = CartItemEntity.Create(1, 1).Value;
            var newQuantity = Create<int>();

            // Act
            cartItem.UpdateQuantity(newQuantity);

            // Assert
            cartItem.Quantity.Value.Should().Be(newQuantity);
        }
    }

    public class ConvertToOrderItem : CartItemEntityTests
    {
        [Fact]
        public void Convert_Successfully()
        {
            // Arrange
            var product = Create<ProductEntity>();
            var productId = Create<int>();
            var cartItem = Create<CartItemEntity>();
            typeof(CartItemEntity)
                .GetProperty(nameof(CartItemEntity.ProductId))!
                .SetValue(cartItem, productId);
            typeof(CartItemEntity)
                .GetProperty(nameof(CartItemEntity.Product))!
                .SetValue(cartItem, product);

            // Act
            var result = cartItem.ConvertToOrderItem();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ProductId.Should().Be(productId);
            result.Value.Quantity.Should().Be(cartItem.Quantity);
            result.Value.Price.Should().Be(product.Price);
        }

        [Fact]
        public void Fail_WhenProductNull()
        {
            // Arrange
            var cartItem = Create<CartItemEntity>();

            // Act
            var result = cartItem.ConvertToOrderItem();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.CriticalError);
        }
    }

    public class TotalPrice : CartItemEntityTests
    {
        [Fact]
        public void CalculateTotalPrice()
        {
            // Arrange
            var product = Create<ProductEntity>();
            var productId = Create<int>();
            var cartItem = Create<CartItemEntity>();
            typeof(CartItemEntity)
                .GetProperty(nameof(CartItemEntity.ProductId))!
                .SetValue(cartItem, productId);
            typeof(CartItemEntity)
                .GetProperty(nameof(CartItemEntity.Product))!
                .SetValue(cartItem, product);
            cartItem.UpdateQuantity(10);

            // Act
            var result = cartItem.TotalPrice;

            // Assert
            result.Should().Be(product.Price.Value * cartItem.Quantity.Value);
        }
    }
}
