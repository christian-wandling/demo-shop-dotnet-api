#region

using DemoShop.Domain.Product.DTOs;
using DemoShop.Domain.Product.Entities;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.Product.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Feature", "Product")]
public class ImageEntityTests : Test
{
    public class Create : ImageEntityTests
    {
        [Fact]
        public void Create_Successfully()
        {
            // Arrange
            var createImageDto = Create<CreateImageDto>();

            // Act
            var result = ImageEntity.Create(createImageDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be(createImageDto.Name);
            result.Value.Uri.Should().Be(createImageDto.Uri);
        }

        [Fact]
        public void Throw_WhenDtoIsNull()
        {
            // Act
            var action = () => ImageEntity.Create(null!);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Throw_WithInvalidName(string invalidName)
        {
            // Arrange
            var dto = Create<CreateImageDto>() with { Name = invalidName };

            // Act
            var action = () => ImageEntity.Create(dto);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Throw_WithInvalidUri()
        {
            // Arrange
            var dto = Create<CreateImageDto>() with { Uri = new Uri("invalid-uri", UriKind.Relative) };

            // Act
            var action = () => ImageEntity.Create(dto);

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
