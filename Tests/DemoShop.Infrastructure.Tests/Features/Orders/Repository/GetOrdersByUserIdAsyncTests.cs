#region

using DemoShop.Domain.Order.Entities;
using DemoShop.Infrastructure.Features.Orders;
using DemoShop.Infrastructure.Tests.Common.Base;
using Serilog;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.Orders.Repository;

[Trait("Feature", "Order")]
public class GetOrdersByUserIdAsyncTests : RepositoryTest
{
    private readonly OrderRepository _sut;

    public GetOrdersByUserIdAsyncTests(ITestOutputHelper output) : base(output)
    {
        var logger = Mock<ILogger>();
        _sut = new OrderRepository(Context, logger);
    }

    [Fact]
    public async Task ShouldReturnOrders_WhenUserIdCorrect()
    {
        // Arrange
        const int userId = 1;
        var orders = Enumerable.Range(0, 3)
            .Select(_ => Create<OrderEntity>())
            .Select(o => OrderEntity.Create(
                userId,
                o.OrderItems
            ).Value)
            .ToList();
        await AddTestDataRangeAsync(orders);

        // Act
        var result = await _sut.GetOrdersByUserIdAsync(userId, CancellationToken.None);
        result = result.ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(o => o.UserId.Should().Be(userId));
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenUserHasNoOrders()
    {
        // Act
        var result = await _sut.GetOrdersByUserIdAsync(999, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ShouldThrow_WhenUserIdIsInvalid(int invalidUserId)
    {
        // Act
        var act = () => _sut.GetOrdersByUserIdAsync(invalidUserId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
