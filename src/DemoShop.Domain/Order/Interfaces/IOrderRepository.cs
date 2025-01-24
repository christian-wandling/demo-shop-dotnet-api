using Ardalis.Result;
using DemoShop.Domain.Order.Entities;

namespace DemoShop.Domain.Order.Interfaces;

public interface IOrderRepository
{
    Task<Result<OrderEntity>> GetOrderByIdAsync(int orderId, string userEmail);
    Task<Result<IEnumerable<OrderEntity>>> GetOrdersByUserEmailAsync(string userEmail);
    Task<Result<OrderEntity>> CreateOrderAsync(OrderEntity orderEntity, string userEmail);
}
