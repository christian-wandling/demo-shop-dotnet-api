#region

using DemoShop.Domain.Order.Entities;
using DemoShop.Infrastructure.Features.Orders;
using DemoShop.Infrastructure.Tests.Common.Base;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.Orders.Repository;

[Trait("Feature", "Order")]
public class CreateOrderAsyncTests : RepositoryTest
{
    private readonly OrderRepository _sut;

    public CreateOrderAsyncTests(ITestOutputHelper output) : base(output)
    {
        _sut = new OrderRepository(Context);
    }

    [Fact]
    public async Task ShouldCreateOrder_WhenOrderEntityCorrect()
    {
        // Arrange
        var order = Create<OrderEntity>();

        // Act
        var result = await _sut.CreateOrderAsync(order, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task ShouldThrow_WhenOrderIdAlreadyExists()
    {
        // Arrange
        var order = Create<OrderEntity>();
        await AddTestDataAsync(order);

        // Act
        var act = () => _sut.CreateOrderAsync(order, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ShouldThrow_WhenOrderEntityNull()
    {
        // Act
        var act = () => _sut.CreateOrderAsync(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ShouldPersistOrderInDatabase_WhenCreated()
    {
        // Arrange
        var order = Create<OrderEntity>();

        // Act
        await _sut.CreateOrderAsync(order, CancellationToken.None);

        // Assert
        var savedOrder = await Context.Set<OrderEntity>()
            .FirstOrDefaultAsync(x => x.Id == order.Id);
        savedOrder.Should().NotBeNull();
        savedOrder.Should().BeEquivalentTo(order);
    }
}
