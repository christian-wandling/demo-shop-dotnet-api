#region

using System.Reflection;
using AutoMapper;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Mappings;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Application.Tests.Features.ShoppingSession.Mappings;

public class ShoppingSessionMappingProfileTests : Test
{
    private readonly MapperConfiguration _configuration;
    private readonly IMapper _mapper;

    public ShoppingSessionMappingProfileTests()
    {
        _configuration = new MapperConfiguration(cfg => { cfg.AddProfile<ShoppingSessionMappingProfile>(); });
        _mapper = _configuration.CreateMapper();
    }

    [Fact]
    public void Configuration_IsValid() =>
        // Assert
        _configuration.AssertConfigurationIsValid();

    [Fact]
    public void Map_ShoppingSessionEntity_To_ShoppingSessionResponse_ShouldMapCorrectly()
    {
        // Arrange
        var productId = Create<int>();
        var entity = Create<ShoppingSessionEntity>();
        var backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(entity, 1);
        entity.AddCartItem(productId);

        // Act
        var result = _mapper.Map<ShoppingSessionResponse>(entity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.UserId.Should().Be(entity.UserId);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public void Map_CartItemEntity_To_CartItemResponse_ShouldMapCorrectly()
    {
        // Arrange
        var product = Create<ProductEntity>();
        var entity = Create<CartItemEntity>();
        typeof(CartItemEntity)
            .GetProperty(nameof(CartItemEntity.ProductId))!
            .SetValue(entity, product.Id);
        typeof(CartItemEntity)
            .GetProperty(nameof(CartItemEntity.Product))!
            .SetValue(entity, product);

        // Act
        var result = _mapper.Map<CartItemResponse>(entity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.ProductId.Should().Be(entity.ProductId);
        result.ProductName.Should().Be(entity.Product?.Name);
        result.ProductThumbnail.Should().Be(entity.Product?.Thumbnail);
        result.Quantity.Should().Be(entity.Quantity.Value);
        result.UnitPrice.Should().Be(entity.Product?.Price.Value);
        result.TotalPrice.Should().Be(entity.TotalPrice);
    }

    [Fact]
    public void Map_CartItemEntity_To_UpdateCartItemQuantityResponse_ShouldMapCorrectly()
    {
        // Arrange
        var entity = Create<CartItemEntity>();

        // Act
        var result = _mapper.Map<UpdateCartItemQuantityResponse>(entity);

        // Assert
        result.Should().NotBeNull();
        result.Quantity.Should().Be(entity.Quantity.Value);
    }

    [Fact]
    public void Map_ShoppingSession_WithNullCartItems_ShouldMapToEmptyCollection()
    {
        // Arrange
        var entity = Create<ShoppingSessionEntity>();

        // Act
        var result = _mapper.Map<ShoppingSessionResponse>(entity);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }
}
