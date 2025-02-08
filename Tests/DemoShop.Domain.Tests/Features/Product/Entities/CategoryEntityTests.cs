#region

using DemoShop.Domain.Product.Entities;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.Product.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Feature", "Product")]
public class CategoryEntityTests : Test
{
    public class Create : CategoryEntityTests
    {
        [Fact]
        public void Create_Successfully()
        {
            // Arrange
            var name = Create<string>();

            // Act
            var result = CategoryEntity.Create(name);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be(name);
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Throw_WithInvalidName(string invalidName)
        {
            // Act
            var action = () => CategoryEntity.Create(invalidName);

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
