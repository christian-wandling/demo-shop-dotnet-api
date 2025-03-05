#region

using DemoShop.Domain.Order.Entities;
using DemoShop.Infrastructure.Features.Orders;
using DemoShop.Infrastructure.Tests.Common.Base;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.Orders.Repository;

[Trait("Feature", "Order")]
public class GetOrderByIdAsyncTests : RepositoryTest
{
    private readonly OrderRepository _sut;

    public GetOrderByIdAsyncTests()
    {
        var logger = Mock<ILogger>();
        _sut = new OrderRepository(Context, logger);
    }

    [Fact]
    public async Task ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var order = Create<OrderEntity>();
        await AddTestDataAsync(order);

        // Act
        var result = await _sut.GetOrderByIdAsync(order.Id, order.UserId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenOrderNotExists()
    {
        // Arrange

        // Act
        var result = await _sut.GetOrderByIdAsync(1, 1, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnNull_WhenUserIdDoesNotMatch()
    {
        // Arrange
        var order = Create<OrderEntity>();
        await AddTestDataAsync(order);
        var wrongUserId = order.UserId + 1;

        // Act
        var result = await _sut.GetOrderByIdAsync(order.Id, wrongUserId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ShouldThrow_WhenOrderIdIsInvalid(int invalidOrderId)
    {
        // Act
        var act = () => _sut.GetOrderByIdAsync(invalidOrderId, 1, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ShouldThrow_WhenUserIdIsInvalid(int invalidUserId)
    {
        // Act
        var act = () => _sut.GetOrderByIdAsync(1, invalidUserId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
