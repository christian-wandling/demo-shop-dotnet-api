#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace DemoShop.Infrastructure.Features.ShoppingSessions.Persistence;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItemEntity>
{
    public void Configure(EntityTypeBuilder<CartItemEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseConfigurations.ConfigureEntity(builder);
        BaseConfigurations.ConfigureAudit(builder);

        builder.ToTable("cart_item");

        builder.HasOne(c => c.ShoppingSession)
            .WithMany(s => s.CartItems)
            .HasForeignKey(c => c.ShoppingSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.Quantity)
            .IsRequired()
            .HasConversion(
                quantity => quantity.Value,
                dbQuantity => Quantity.Create(dbQuantity)
            );

        builder.HasIndex(c => new { c.ShoppingSessionId, c.ProductId })
            .IsUnique();
    }
}
