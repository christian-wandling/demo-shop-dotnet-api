#region

using System.Reflection;
using Ardalis.Result;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Enums;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.Order.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Feature", "Order")]
public class OrderEntityTests : Test
{
    public class Create : OrderEntityTests
    {
        [Fact]
        public void Create_WithValidInputs_ShouldReturnSuccessResult()
        {
            // Arrange
            var userId = Create<int>();
            var items = Create<List<OrderItemEntity>>();

            // Act
            var result = OrderEntity.Create(userId, items);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.UserId.Should().Be(userId);
            result.Value.OrderItems.Should().BeEquivalentTo(items);
            result.Value.Status.Should().Be(OrderStatus.Created);
        }

        [Fact]
        public void Create_WithNegativeUserId_ShouldThrowException()
        {
            // Arrange
            const int userId = -1;
            var items = Create<List<OrderItemEntity>>();

            // Act
            var act = () => OrderEntity.Create(userId, items);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullItems_ShouldThrowException()
        {
            // Arrange
            var userId = Create<int>();

            // Act
            var act = () => OrderEntity.Create(userId, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }

    public class AddOrderItem : OrderEntityTests
    {
        [Fact]
        public void AddOrderItem_WithNewItem_ShouldSucceed()
        {
            // Arrange
            var userId = Create<int>();
            var items = Create<List<OrderItemEntity>>();
            var order = OrderEntity.Create(userId, items).Value;
            var newItem = Create<OrderItemEntity>();
            var backingField = typeof(OrderItemEntity)
                .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            backingField?.SetValue(newItem, items.Max(i => i.Id) + 1);

            // Act
            var result = order.AddOrderItem(newItem);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.OrderItems.Should().Contain(newItem);
        }

        [Fact]
        public void AddOrderItem_WithDuplicateItem_ShouldReturnError()
        {
            // Arrange
            var item = Create<OrderItemEntity>();
            var order = OrderEntity.Create(Create<int>(), new List<OrderItemEntity> { item }).Value;

            // Act
            var result = order.AddOrderItem(item);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Conflict);
        }
    }

    public class Amount : OrderEntityTests
    {
        [Fact]
        public void Amount_ShouldCalculateTotalCorrectly()
        {
            // Arrange
            var items = new List<OrderItemEntity> { Create<OrderItemEntity>(), Create<OrderItemEntity>() };
            var order = OrderEntity.Create(Create<int>(), items).Value;

            // Act
            var amount = order.Amount;

            // Assert
            amount.Value.Should().Be(items.Sum(i => i.TotalPrice.Value));
        }
    }
}
