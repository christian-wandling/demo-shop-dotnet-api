using Ardalis.GuardClauses;
using DemoShop.Domain.Order.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Orders.Persistence;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItemEntity>
{
    public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseConfigurations.ConfigureEntity(builder);
        BaseConfigurations.ConfigureAudit(builder);
        BaseConfigurations.ConfigureSoftDelete(builder);

        builder.ToTable("OrderItem");

        builder.HasOne(o => o.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(o => o.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.ProductId)
            .IsRequired();

        builder.Property(o => o.ProductName)
            .IsRequired();

        builder.Property(o => o.ProductThumbnail)
            .IsRequired();

        builder.Property(o => o.Quantity)
            .IsRequired();

        builder.Property(o => o.Price)
            .IsRequired();
    }
}
