#region

using AutoMapper;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Mappings;
using DemoShop.Domain.Product.Entities;
using DemoShop.TestUtils.Common.Base;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Application.Tests.Features.Product.Mappings;

[Trait("Feature", "Product")]
public class ProductMappingProfileTests : Test
{
    private readonly IMapper _mapper;

    public ProductMappingProfileTests(ITestOutputHelper? output = null) : base(output)
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMappingProfile>());
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Configuration_ShouldBeValid()
    {
        // Assert
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMappingProfile>());
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_ProductEntity_To_ProductResponse_ShouldMapAllProperties()
    {
        // Arrange
        var source = Create<ProductEntity>();

        // Act
        var result = _mapper.Map<ProductResponse>(source);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(source.Id);
        result.Name.Should().Be(source.Name);
        result.Description.Should().Be(source.Description);
        result.Price.Should().Be(source.Price.Value);
        result.Categories.Should().BeEquivalentTo(source.Categories.Select(c => c.ToString()));
        result.Images.Should().BeEquivalentTo(source.Images.Select(i => _mapper.Map<ImageResponse>(i)));
        result.Thumbnail.Should().BeEquivalentTo(_mapper.Map<ImageResponse>(source.Images.FirstOrDefault()));
    }

    [Fact]
    public void Map_ProductEntity_To_ProductResponse_WithEmptyImages_ShouldHaveNullThumbnail()
    {
        // Arrange
        var source = Create<ProductEntity>();

        // Act
        var result = _mapper.Map<ProductResponse>(source);

        // Assert
        result.Should().NotBeNull();
        result.Thumbnail.Should().BeNull();
        result.Images.Should().BeEmpty();
    }

    [Fact]
    public void Map_ProductEntityCollection_To_ProductListResponse_ShouldMapAllItems()
    {
        // Arrange
        var sources = Create<List<ProductEntity>>();

        // Act
        var result = _mapper.Map<ProductListResponse>(sources);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(sources.Count);
        result.Items.Should().BeEquivalentTo(
            sources.Select(s => _mapper.Map<ProductResponse>(s)));
    }

    [Fact]
    public void Map_EmptyProductEntityCollection_To_ProductListResponse_ShouldReturnEmptyList()
    {
        // Arrange
        var sources = new List<ProductEntity>();

        // Act
        var result = _mapper.Map<ProductListResponse>(sources);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public void Map_CategoryEntity_To_String_ShouldReturnStringRepresentation()
    {
        // Arrange
        var source = Create<CategoryEntity>();

        // Act
        var result = _mapper.Map<string>(source);

        // Assert
        result.Should().Be(source.Name);
    }

    [Fact]
    public void Map_ImageEntity_To_ImageResponse_ShouldMapCorrectly()
    {
        // Arrange
        var source = Create<ImageEntity>();

        // Act
        var result = _mapper.Map<ImageResponse>(source);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(source, options =>
            options.ExcludingMissingMembers());
    }

    [Fact]
    public void Map_NullImageEntity_To_ImageResponse_ShouldReturnNull()
    {
        // Arrange
        ImageEntity? source = null;

        // Act
        var result = _mapper.Map<ImageResponse>(source);

        // Assert
        result.Should().BeNull();
    }
}
