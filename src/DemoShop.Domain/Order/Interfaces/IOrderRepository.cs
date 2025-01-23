using Ardalis.Result;

namespace DemoShop.Domain.Order.Interfaces;

public interface IOrderRepository
{
    Task<Result<Entities.OrderEntity>> GetOrderByIdAsync(int orderId, string userEmail);
    Task<Result<IEnumerable<Entities.OrderEntity>>> GetOrdersByUserEmailAsync(string userEmail);
    Task<Result<Entities.OrderEntity>> CreateOrderAsync(Entities.OrderEntity orderEntity, string userEmail);
}
