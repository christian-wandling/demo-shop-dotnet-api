#region

using System.Reflection;
using Ardalis.Result;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.ShoppingSession;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Feature", "ShoppingSession")]
public class ShoppingSessionEntityTests : Test
{
    private readonly ShoppingSessionEntity _sut;

    private ShoppingSessionEntityTests()
    {
        _sut = Create<ShoppingSessionEntity>();

        var backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(_sut, Create<int>());
    }

    public class Create : ShoppingSessionEntityTests
    {
        [Fact]
        public void Create_Successfully()
        {
            // Arrange
            var userId = Create<int>();

            // Act
            var result = ShoppingSessionEntity.Create(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.UserId.Should().Be(userId);
            result.Value.CartItems.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Fail_With_Invalid_UserId(int invalidUserId)
        {
            // Act
            var action = () => ShoppingSessionEntity.Create(invalidUserId);

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }

    public class AddCartItem : ShoppingSessionEntityTests
    {
        [Fact]
        public void Add_CartItem_Successfully()
        {
            // Arrange

            var productId = Create<int>();

            // Act
            var result = _sut.AddCartItem(productId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _sut.CartItems.Should().ContainSingle();
            _sut.CartItems.First().ProductId.Should().Be(productId);
        }

        [Fact]
        public void Fail_When_Product_Already_In_Cart()
        {
            // Arrange
            var productId = Create<int>();
            _sut.AddCartItem(productId);

            // Act
            var result = _sut.AddCartItem(productId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Conflict);
        }
    }

    public class UpdateCartItem : ShoppingSessionEntityTests
    {
        [Fact]
        public void Update_Quantity_Successfully()
        {
            // Arrange
            var productId = Create<int>();
            var cartItem = _sut.AddCartItem(productId).Value;
            var newQuantity = Create<int>();

            // Act
            var result = _sut.UpdateCartItem(cartItem.Id, newQuantity);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Quantity.Value.Should().Be(newQuantity);
        }

        [Fact]
        public void Fail_When_CartItem_Not_Found()
        {
            // Arrange
            var invalidCartItemId = Create<int>();
            var quantity = Create<int>();

            // Act
            var result = _sut.UpdateCartItem(invalidCartItemId, quantity);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
        }
    }

    public class RemoveCartItem : ShoppingSessionEntityTests
    {
        [Fact]
        public void Remove_CartItem_Successfully()
        {
            // Arrange
            var productId = Create<int>();
            var cartItem = _sut.AddCartItem(productId).Value;

            // Act
            var result = _sut.RemoveCartItem(cartItem.Id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _sut.CartItems.Should().BeEmpty();
        }

        [Fact]
        public void Fail_When_CartItem_Not_Found()
        {
            // Arrange
            var invalidCartItemId = Create<int>();

            // Act
            var result = _sut.RemoveCartItem(invalidCartItemId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
        }
    }

    public class ConvertToOrder : ShoppingSessionEntityTests
    {
        [Fact]
        public void Convert_To_Order_Successfully()
        {
            // Arrange
            _sut.AddCartItem(1);

            // Act
            var result = _sut.ConvertToOrder();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.UserId.Should().Be(_sut.UserId);
            result.Value.OrderItems.Should().HaveCount(_sut.CartItems.Count);
        }

        [Fact]
        public void Fail_When_No_Items_In_Cart()
        {
            // Act
            var result = _sut.ConvertToOrder();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
        }
    }
}
