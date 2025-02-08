#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

#endregion

namespace DemoShop.Infrastructure.Features.Orders;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<OrderEntity?> GetOrderByIdAsync(int orderId, int userId, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(orderId, nameof(orderId));
        Guard.Against.NegativeOrZero(userId, nameof(userId));

        return await context.Query<OrderEntity>()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<OrderEntity>> GetOrdersByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(userId, nameof(userId));

        return await context.Query<OrderEntity>()
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderEntity?> CreateOrderAsync(OrderEntity orderEntity, CancellationToken cancellationToken)
    {
        Guard.Against.Null(orderEntity, nameof(orderEntity));

        var created = context.Set<OrderEntity>().Add(orderEntity);
        await context.SaveChangesAsync(cancellationToken);

        return created.Entity;
    }
}
