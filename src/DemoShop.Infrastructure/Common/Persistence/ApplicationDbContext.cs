using System.Globalization;
using Ardalis.GuardClauses;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DemoShop.Infrastructure.Common.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        => Set<TEntity>();

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

    public override int SaveChanges() => throw new InvalidOperationException("Use SaveChangesAsync instead");
}
