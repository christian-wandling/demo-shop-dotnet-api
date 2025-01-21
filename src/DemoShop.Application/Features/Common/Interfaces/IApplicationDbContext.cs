using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Session.Entities;
using DemoShop.Domain.User.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoShop.Application.Features.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<UserEntity> Users { get; }
    DbSet<AddressEntity> Addresses { get; }
    DbSet<ProductEntity> Products { get; }
    DbSet<ImageEntity> Images { get; }
    DbSet<CategoryEntity> Categories { get; }
    DbSet<ShoppingSessionEntity> ShoppingSessions { get; }
    DbSet<CartItemEntity> CartItems { get; }
    DbSet<OrderEntity> Orders { get; }
    DbSet<OrderItemEntity> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
