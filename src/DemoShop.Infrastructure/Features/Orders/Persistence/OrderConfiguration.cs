using Ardalis.GuardClauses;
using DemoShop.Domain.Order.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Orders.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseConfigurations.ConfigureEntity(builder);
        BaseConfigurations.ConfigureAudit(builder);
        BaseConfigurations.ConfigureSoftDelete(builder);

        builder.ToTable("Order");

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.OrderItems)
            .WithOne(o => o.Order)
            .HasForeignKey(o => o.OrderId);

        builder.Property(o => o.Status)
            .IsRequired();
    }
}
