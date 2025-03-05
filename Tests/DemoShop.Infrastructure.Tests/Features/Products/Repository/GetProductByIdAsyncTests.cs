#region

using DemoShop.Domain.Product.Entities;
using DemoShop.Infrastructure.Features.Products;
using DemoShop.Infrastructure.Tests.Common.Base;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.Products.Repository;

[Trait("Feature", "Product")]
public class GetProductByIdAsyncTests : RepositoryTest
{
    private readonly ProductRepository _sut;

    public GetProductByIdAsyncTests()
    {
        var logger = Mock<ILogger>();
        _sut = new ProductRepository(Context, logger);
    }

    [Fact]
    public async Task ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = Create<ProductEntity>();
        await AddTestDataAsync(product);

        // Act
        var result = await _sut.GetProductByIdAsync(product.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenProductNotExists()
    {
        // Arrange

        // Act
        var result = await _sut.GetProductByIdAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ShouldThrow_WhenProductIdIsInvalid(int invalidProductId)
    {
        // Act
        var act = () => _sut.GetProductByIdAsync(invalidProductId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
