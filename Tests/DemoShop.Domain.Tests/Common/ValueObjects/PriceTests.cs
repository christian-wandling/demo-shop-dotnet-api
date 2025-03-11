#region

using DemoShop.Domain.Common.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Common.ValueObjects;

[Trait("Feature", "Common")]
public class PriceTests : Test
{
    [Fact]
    public void Empty_ReturnsZeroPrice()
    {
        // Act
        var price = Price.Empty;

        // Assert
        price.Value.Should().Be(0);
    }

    [Theory]
    [InlineData(10.99)]
    [InlineData(0.01)]
    [InlineData(999.99)]
    public void Create_WithValidPrice_CreatesPrice(decimal value)
    {
        // Act
        var price = Price.Create(value);

        // Assert
        price.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Create_WithNegativePrice_ThrowsException(decimal value)
    {
        // Act
        var action = () => Price.Create(value);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(10.00, 2, 20.00)]
    [InlineData(10.99, 3, 32.97)]
    [InlineData(0.01, 5, 0.05)]
    public void Multiply_ReturnsCorrectResult(decimal initial, int multiplier, decimal expected)
    {
        // Arrange
        var price = Price.Create(initial);

        // Act
        var result = price.Multiply(multiplier);

        // Assert
        result.Value.Should().Be(expected);
    }

    [Fact]
    public void EqualityComparison_WithSameValues_ReturnsTrue()
    {
        // Arrange
        var price1 = Price.Create(10.99m);
        var price2 = Price.Create(10.99m);

        // Assert
        price1.Should().Be(price2);
    }

    [Fact]
    public void EqualityComparison_WithDifferentValues_ReturnsFalse()
    {
        // Arrange
        var price1 = Price.Create(10.99m);
        var price2 = Price.Create(11.99m);

        // Assert
        price1.Should().NotBe(price2);
    }
}
