using System.Globalization;
using Ardalis.GuardClauses;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Enums;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Session.Entities;
using DemoShop.Domain.User.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DemoShop.Infrastructure.Common.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    private const string UpdatedAtColumn = "UpdatedAt";
    private const string DeletedAtProperty = nameof(EntitySoftDelete.DeletedAt);

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<AddressEntity> Addresses => Set<AddressEntity>();

    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<ImageEntity> Images => Set<ImageEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();

    public DbSet<ShoppingSessionEntity> ShoppingSessions => Set<ShoppingSessionEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();

    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Guard.Against.Null(modelBuilder, nameof(modelBuilder));
        base.OnModelCreating(modelBuilder);
        ConfigureBaseEntities(modelBuilder);
        ConfigureEnums(modelBuilder);
    }

    private static void ConfigureBaseEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entity>()
            .Property(e => e.ModifiedAt)
            .HasColumnName(UpdatedAtColumn);

        modelBuilder.Entity<EntitySoftDelete>()
            .HasQueryFilter(e => !e.Deleted);
    }


    private static void ConfigureEnums(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<OrderEntity>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString().ToUpper(CultureInfo.InvariantCulture),
                v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v, true)
            );

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        var entries = ChangeTracker.Entries<Entity>();

        foreach (var entry in entries) UpdateEntityTimestamps(entry, utcNow);

        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges() => throw new InvalidOperationException("Use SaveChangesAsync instead");

    private static void UpdateEntityTimestamps(EntityEntry<Entity> entry, DateTime utcNow)
    {
        switch (entry)
        {
            case { State: EntityState.Added }:
                SetCreatedTimestamps(entry, utcNow);
                break;
            case { State: EntityState.Modified }:
                SetModifiedTimestamps(entry, utcNow);
                break;
        }
    }

    private static void SetCreatedTimestamps(EntityEntry<Entity> entry, DateTime utcNow)
    {
        entry.Property(e => e.CreatedAt).CurrentValue = utcNow;
        entry.Property(e => e.ModifiedAt).CurrentValue = utcNow;
    }

    private static void SetModifiedTimestamps(EntityEntry<Entity> entry, DateTime utcNow)
    {
        entry.Property(e => e.ModifiedAt).CurrentValue = utcNow;

        if (entry.Entity is EntitySoftDelete { Deleted: true, DeletedAt: null })
            entry.Property(DeletedAtProperty).CurrentValue = utcNow;
    }
}
