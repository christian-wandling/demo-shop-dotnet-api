using DemoShop.Domain.Order.Entities;

namespace DemoShop.Domain.Order.Interfaces;

public interface IOrderRepository
{
    Task<OrderEntity?> GetOrderByIdAsync(int orderId, int userId, CancellationToken cancellationToken);
    Task<IEnumerable<OrderEntity>> GetOrdersByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<OrderEntity?> CreateOrderAsync(OrderEntity orderEntity, CancellationToken cancellationToken);
}
