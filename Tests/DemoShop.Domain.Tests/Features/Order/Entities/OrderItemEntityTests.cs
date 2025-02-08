#region

using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.Order.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Feature", "Order")]
public class OrderItemEntityTests : Test
{
    public class Create : OrderItemEntityTests
    {
        [Fact]
        public void Create_WithValidInputs_ReturnsSuccessResult()
        {
            // Arrange
            var productId = Create<int>();
            var product = Create<OrderProduct>();
            var quantity = Create<Quantity>();
            var price = Create<Price>();

            // Act
            var result = OrderItemEntity.Create(productId, product, quantity, price);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithNegativeProductId_ThrowsArgumentException()
        {
            // Arrange
            var productId = -1;
            var product = Create<OrderProduct>();
            var quantity = Create<Quantity>();
            var price = Create<Price>();

            // Act
            var action = () => OrderItemEntity.Create(productId, product, quantity, price);

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }

    public class TotalPrice : OrderItemEntityTests
    {
        [Fact]
        public void TotalPrice_CalculatesCorrectly()
        {
            // Arrange
            var quantity = Quantity.Create(2);
            var price = Price.Create(new decimal(100));
            var orderItem = OrderItemEntity.Create(
                Create<int>(),
                Create<OrderProduct>(),
                quantity,
                price
            ).Value;

            // Act
            var totalPrice = orderItem.TotalPrice;

            // Assert
            totalPrice.Value.Should().Be(200);
        }
    }
}
