#region

using DemoShop.Domain.Order.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.Order.ValueObjects;

[Trait("Feature", "Product")]
public class OrderProductTests : Test
{
    [Fact]
    public void Empty_ShouldReturnEmptyOrderProduct()
    {
        // Act
        var result = OrderProduct.Empty;

        // Assert
        result.ProductName.Should().BeEmpty();
        result.ProductThumbnail.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithValidInputs_ShouldCreateOrderProduct()
    {
        // Arrange
        var name = Create<string>();
        var thumbnail = Create<string>();

        // Act
        var result = OrderProduct.Create(name, thumbnail);

        // Assert
        result.ProductName.Should().Be(name);
        result.ProductThumbnail.Should().Be(thumbnail);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidProductName_ShouldThrowException(string invalidName)
    {
        // Arrange
        var thumbnail = Create<string>();

        // Act
        var act = () => OrderProduct.Create(invalidName, thumbnail);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equal_OrderProducts_ShouldBeEqual()
    {
        // Arrange
        var orderProduct1 = OrderProduct.Create("productName1", "http://my-url/thumbnail-1");
        var orderProduct2 = OrderProduct.Create("productName1", "http://my-url/thumbnail-1");

        // Assert
        orderProduct1.Should().Be(orderProduct2);
        orderProduct1.GetHashCode().Should().Be(orderProduct2.GetHashCode());
    }

    [Theory]
    [InlineData("productName1", "http://my-url/thumbnail-1", "productname2", "http://my-url/thumbnail-1")]
    [InlineData("productName1", "http://my-url/thumbnail-1", "productName1", "http://my-url/thumbnail-2")]
    public void Different_OrderProducts_ShouldNotBeEqual(
        string productName1, string productThumbnail1,
        string productName2, string productThumbnail2)
    {
        // Arrange
        var orderProduct1 = OrderProduct.Create(productName1, productThumbnail1);
        var orderProduct2 = OrderProduct.Create(productName2, productThumbnail2);

        // Assert
        orderProduct1.Should().NotBe(orderProduct2);
        orderProduct1.GetHashCode().Should().NotBe(orderProduct2.GetHashCode());
    }

    [Fact]
    public void Equal_OrderProducts_ShouldHaveSameHashCode()
    {
        // Arrange
        var orderProduct1 = OrderProduct.Create("productName1", "http://my-url/thumbnail-1");
        var orderProduct2 = OrderProduct.Create("productName1", "http://my-url/thumbnail-1");

        // Act
        var hashCode1 = orderProduct1.GetHashCode();
        var hashCode2 = orderProduct2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }
}
