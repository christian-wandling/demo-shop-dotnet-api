#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Exceptions;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Common.Guards;

[Trait("Feature", "Common")]
public class PriceGuardTests : Test
{
    [Theory]
    [InlineData(1.00)]
    [InlineData(99.99)]
    [InlineData(100.00)]
    [InlineData(999999999999.99)]
    public void InvalidPrice_WithValidPrice_ReturnsInput(decimal input)
    {
        // Act
        var result = Guard.Against.InvalidPrice(input);

        // Assert
        result.Should().Be(input);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99.99)]
    public void InvalidPrice_WithNegativeOrZero_ThrowsArgumentException(decimal input)
    {
        // Act
        var action = () => Guard.Against.InvalidPrice(input);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(10.999)]
    [InlineData(10.9)]
    [InlineData(10.001)]
    public void InvalidPrice_WithInvalidDecimalPlaces_ThrowsInvalidPriceDomainException(decimal input)
    {
        // Act
        var action = () => Guard.Against.InvalidPrice(input);

        // Assert
        action.Should().Throw<InvalidPriceDomainException>();
    }

    [Fact]
    public void InvalidPrice_WithParameterName_IncludesInException()
    {
        // Arrange
        const string paramName = "testPrice";
        const decimal invalidPrice = 0;

        // Act
        var action = () => Guard.Against.InvalidPrice(invalidPrice, paramName);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName(paramName);
    }
}
