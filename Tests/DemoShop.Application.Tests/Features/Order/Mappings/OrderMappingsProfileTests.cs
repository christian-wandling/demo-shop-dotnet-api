#region

using AutoMapper;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Order.Mappings;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Product.Entities;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Application.Tests.Features.Order.Mappings;

public class OrderMappingProfileTests : Test
{
    private readonly IMapper _mapper;

    public OrderMappingProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<OrderMappingProfile>());

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void OrderEntity_To_OrderResponse_ShouldMapCorrectly()
    {
        // Arrange
        var orderEntity = Create<OrderEntity>();

        // Act
        var result = _mapper.Map<OrderResponse>(orderEntity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(orderEntity.Id);
        result.UserId.Should().Be(orderEntity.UserId);
        result.Amount.Should().Be(orderEntity.Amount.Value);
        result.Status.Should().Be(orderEntity.Status);
        result.Created.Should().Be(orderEntity.Audit.CreatedAt);
        result.Items.Should().HaveCount(orderEntity.OrderItems.Count);
    }

    [Fact]
    public void OrderItemEntity_To_OrderItemResponse_ShouldMapCorrectly()
    {
        // Arrange
        var orderItemEntity = Create<OrderItemEntity>();

        // Act
        var result = _mapper.Map<OrderItemResponse>(orderItemEntity);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(orderItemEntity.ProductId);
        result.ProductName.Should().Be(orderItemEntity.Product.ProductName);
        result.ProductThumbnail.Should().Be(orderItemEntity.Product.ProductThumbnail);
        result.Quantity.Should().Be(orderItemEntity.Quantity.Value);
        result.UnitPrice.Should().Be(orderItemEntity.Price.Value);
        result.TotalPrice.Should().Be(orderItemEntity.TotalPrice.Value);
    }

    [Fact]
    public void OrderEntityCollection_To_OrderListResponse_ShouldMapCorrectly()
    {
        // Arrange
        var orderEntities = Create<IReadOnlyCollection<OrderEntity>>();

        // Act
        var result = _mapper.Map<OrderListResponse>(orderEntities);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(orderEntities.Count);
    }


    [Fact]
    public void Configuration_ShouldBeValid()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<OrderMappingProfile>());

        // Act & Assert
        config.AssertConfigurationIsValid();
    }
}
