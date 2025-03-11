#region

using DemoShop.Domain.Product.Entities;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.Product.Entities;

[Trait("Product", "Unit")]
[Trait("Feature", "Product")]
public class ProductEntityTests : Test
{
    public class Create : ProductEntityTests
    {
        [Fact]
        public void Create_Successfully()
        {
            // Arrange
            var name = Create<string>();
            var description = Create<string>();
            var price = Create<decimal>();

            // Act
            var result = ProductEntity.Create(name, description, price);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be(name);
            result.Value.Description.Should().Be(description);
            result.Value.Price.Value.Should().Be(price);
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Throw_WithInvalidName(string invalidName)
        {
            // Arrange
            var description = Create<string>();
            var price = Create<decimal>();

            // Act
            var action = () => ProductEntity.Create(invalidName, description, price);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Throw_WithInvalidDescription(string invalidDescription)
        {
            // Arrange
            var name = Create<string>();
            var price = Create<decimal>();

            // Act
            var action = () => ProductEntity.Create(name, invalidDescription, price);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        public void Throw_WithInvalidPrice(decimal invalidPrice)
        {
            // Arrange
            var name = Create<string>();
            var description = Create<string>();

            // Act
            var action = () => ProductEntity.Create(name, description, invalidPrice);

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
