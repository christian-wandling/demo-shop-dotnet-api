using Ardalis.GuardClauses;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.ShoppingSessions.Persistence;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItemEntity>
{
    public void Configure(EntityTypeBuilder<CartItemEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseEntityConfiguration.Configure(builder);
        BaseEntityConfiguration.ConfigureAudit(builder);

        builder.ToTable("CartItem");

        builder.HasOne(c => c.ShoppingSession)
            .WithMany(s => s.CartItems)
            .HasForeignKey(c => c.ShoppingSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.Quantity)
            .IsRequired();
    }
}
