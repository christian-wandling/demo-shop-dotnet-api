using DemoShop.Domain.Orders.Entities;
using DemoShop.Domain.Products.Entities;
using DemoShop.Domain.Sessions.Entities;
using DemoShop.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoShop.Application.Features.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Address> Addresses { get; }
    DbSet<Product> Products { get; }
    DbSet<Image> Images { get; }
    DbSet<Category> Categories { get; }
    DbSet<ShoppingSession> ShoppingSessions { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
