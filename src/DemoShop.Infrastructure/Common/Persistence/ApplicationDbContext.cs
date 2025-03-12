#region

using Ardalis.GuardClauses;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Domain.Order.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

#endregion

namespace DemoShop.Infrastructure.Common.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHostEnvironment? env = null)
    : DbContext(options), IApplicationDbContext
{
    public IQueryable<TEntity> Query<TEntity>() where TEntity : class => Set<TEntity>();

    public override int SaveChanges() => throw new InvalidOperationException("Use SaveChangesAsync instead");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Guard.Against.Null(optionsBuilder, nameof(optionsBuilder));
        base.OnConfiguring(optionsBuilder);

        optionsBuilder
            .UseSnakeCaseNamingConvention();

        if (env?.IsDevelopment() == true)
        {
            optionsBuilder
                .LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Guard.Against.Null(modelBuilder, nameof(modelBuilder));

        modelBuilder.HasPostgresEnum<OrderStatus>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
