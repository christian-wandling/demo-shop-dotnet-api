using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using DemoShop.Infrastructure.Common;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DemoShop.Infrastructure.Features.Orders;

public class OrderRepository(ApplicationDbContext context, ILogger<OrderRepository> logger)
    : Repository<OrderEntity>(context, logger), IOrderRepository
{
    public async Task<Result<OrderEntity>> GetOrderByIdAsync(int orderId, string userEmail)
    {
        Guard.Against.NegativeOrZero(orderId, nameof(orderId));
        Guard.Against.NullOrEmpty(userEmail, nameof(userEmail));

        try
        {
            var order = await context.Query<OrderEntity>()
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.User!.Email.Value == userEmail)
                .ConfigureAwait(false);

            return order is not null
                ? Result<OrderEntity>.Success(order)
                : HandleNotFound<OrderEntity>("GetProductByIdAsync", $"{orderId}");
        }
        catch (DbException ex)
        {
            return HandleDbException<OrderEntity>("GetProductByIdAsync", ex, $"{orderId}");
        }
        catch (InvalidOperationException ex)
        {
            return HandleInvalidOperationException<OrderEntity>("GetProductByIdAsync", ex, $"{orderId}");
        }
    }

    public async Task<Result<IEnumerable<OrderEntity>>> GetOrdersByUserEmailAsync(string userEmail)
    {
        Guard.Against.NullOrEmpty(userEmail, nameof(userEmail));

        try
        {
            return await context.Query<OrderEntity>()
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .Where(o => o.User!.Email.Value == userEmail)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (DbException ex)
        {
            return HandleDbException<IEnumerable<OrderEntity>>("GetOrdersByUserEmailAsync", ex);
        }
        catch (InvalidOperationException ex)
        {
            return HandleInvalidOperationException<IEnumerable<OrderEntity>>("GetOrdersByUserEmailAsync", ex);
        }
    }

    public async Task<Result<OrderEntity>> CreateOrderAsync(OrderEntity orderEntity, string userEmail)
    {
        Guard.Against.Null(orderEntity, nameof(orderEntity));
        Guard.Against.NullOrEmpty(userEmail, nameof(userEmail));

        try
        {
            var created = context.Set<OrderEntity>().Add(orderEntity);
            await context.SaveChangesAsync().ConfigureAwait(false);

            return await GetOrderByIdAsync(created.Entity.Id, userEmail).ConfigureAwait(false);
        }
        catch (DbException ex)
        {
            logger.LogDatabaseError<OrderEntity>("CreateOrderAsync", ex, userEmail);
            return Result<OrderEntity>.Error("Database operation failed");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogOperationError<OrderEntity>("CreateOrderAsync", ex, userEmail);
            return Result<OrderEntity>.Error("Operation failed");
        }
    }
}
