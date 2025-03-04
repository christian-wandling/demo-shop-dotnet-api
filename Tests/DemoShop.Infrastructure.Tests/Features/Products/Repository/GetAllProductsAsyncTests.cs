#region

using DemoShop.Domain.Product.Entities;
using DemoShop.Infrastructure.Features.Products;
using DemoShop.Infrastructure.Tests.Common.Base;
using Serilog;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.Products.Repository;

[Trait("Feature", "Product")]
public class GetAllProductsAsyncTests : RepositoryTest
{
    private readonly ProductRepository _sut;

    public GetAllProductsAsyncTests(ITestOutputHelper output) : base(output)
    {
        var logger = Mock<ILogger>();
        _sut = new ProductRepository(Context, logger);
    }

    [Fact]
    public async Task ShouldReturnProducts_WhenProductsExist()
    {
        // Arrange
        var products = Enumerable.Range(0, 3)
            .Select(_ => Create<ProductEntity>())
            .ToList();
        await AddTestDataRangeAsync(products);

        // Act
        var result = await _sut.GetAllProductsAsync(CancellationToken.None);
        result = result.ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoProducts()
    {
        // Act
        var result = await _sut.GetAllProductsAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
