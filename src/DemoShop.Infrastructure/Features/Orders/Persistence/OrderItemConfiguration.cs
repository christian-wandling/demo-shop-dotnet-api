#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace DemoShop.Infrastructure.Features.Orders.Persistence;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItemEntity>
{
    public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseConfigurations.ConfigureEntity(builder);
        BaseConfigurations.ConfigureAudit(builder);
        BaseConfigurations.ConfigureSoftDelete(builder);

        builder.ToTable("order_item");

        builder.HasOne(o => o.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(o => o.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.OrderId)
            .IsRequired();

        builder.OwnsOne(o => o.Product, navigationBuilder =>
        {
            navigationBuilder.WithOwner().HasForeignKey("id");

            navigationBuilder.Property(o => o.ProductName)
                .HasColumnName("product_name");

            navigationBuilder.Property(o => o.ProductThumbnail)
                .HasColumnName("product_thumbnail");
        });

        builder.Property(o => o.Quantity)
            .IsRequired()
            .HasConversion(
                quantity => quantity.Value,
                dbQuantity => Quantity.Create(dbQuantity)
            );

        builder.Property(o => o.Price)
            .IsRequired()
            .HasConversion(
                price => price.Value,
                dbPrice => Price.Create(dbPrice)
            );

        builder.HasIndex(o => new { o.OrderId, o.ProductId })
            .IsUnique();
    }
}
