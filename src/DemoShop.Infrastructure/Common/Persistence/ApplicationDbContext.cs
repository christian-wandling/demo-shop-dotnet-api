using System.Globalization;
using Ardalis.GuardClauses;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Domain.Common.Interfaces;
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
    private const string DeletedAtProperty = nameof(ISoftDeletable.DeletedAt);

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<AddressEntity> Addresses => Set<AddressEntity>();

    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<ImageEntity> Images => Set<ImageEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();

    public DbSet<ShoppingSessionEntity> ShoppingSessions => Set<ShoppingSessionEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();

    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Guard.Against.Null(optionsBuilder, nameof(optionsBuilder));
        base.OnConfiguring(optionsBuilder);

        optionsBuilder
            .LogTo(Console.WriteLine) // Log EF SQL queries to Console.
            .EnableSensitiveDataLogging(); // Log sensitive data like parameters.
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Guard.Against.Null(modelBuilder, nameof(modelBuilder));

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ConfigureEnums(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        Guard.Against.Null(configurationBuilder, nameof(configurationBuilder));

        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Conventions.Add(_ => new LowerCasePropertiesConvention());
        configurationBuilder.Conventions.Add(_ => new ModifiedAtPropertyConvention());
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
        var entries = ChangeTracker.Entries<IEntity>();

        foreach (var entry in entries) UpdateEntityTimestamps(entry, utcNow);

        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges() => throw new InvalidOperationException("Use SaveChangesAsync instead");

    private static void UpdateEntityTimestamps(EntityEntry<IEntity> entry, DateTime utcNow)
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

    private static void SetCreatedTimestamps(EntityEntry<IEntity> entry, DateTime utcNow)
    {
        entry.Property(e => e.CreatedAt).CurrentValue = utcNow;
        entry.Property(e => e.ModifiedAt).CurrentValue = utcNow;
    }

    private static void SetModifiedTimestamps(EntityEntry<IEntity> entry, DateTime utcNow)
    {
        entry.Property(e => e.ModifiedAt).CurrentValue = utcNow;

        if (entry.Entity is ISoftDeletable { Deleted: true, DeletedAt: null })
            entry.Property(DeletedAtProperty).CurrentValue = utcNow;
    }
}
