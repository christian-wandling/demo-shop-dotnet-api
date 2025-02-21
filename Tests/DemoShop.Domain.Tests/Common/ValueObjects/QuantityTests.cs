#region

using DemoShop.Domain.Common.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Common.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Feature", "Common")]
public class QuantityTests : Test
{
    [Fact]
    public void Create_WithValidQuantity_CreatesQuantity()
    {
        // Act
        var price = Quantity.Create(1);

        // Assert
        price.Value.Should().Be(1);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Create_WithInvalidQuantity_ThrowsException(int invalidValue)
    {
        // Act
        var action = () => Quantity.Create(invalidValue);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void EqualityComparison_WithSameValues_ReturnsTrue()
    {
        // Arrange
        var price1 = Quantity.Create(1);
        var price2 = Quantity.Create(1);

        // Assert
        price1.Should().Be(price2);
    }

    [Fact]
    public void EqualityComparison_WithDifferentValues_ReturnsFalse()
    {
        // Arrange
        var price1 = Quantity.Create(1);
        var price2 = Quantity.Create(2);

        // Assert
        price1.Should().NotBe(price2);
    }
}
