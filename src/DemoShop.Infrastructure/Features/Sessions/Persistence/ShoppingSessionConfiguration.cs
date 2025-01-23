using Ardalis.GuardClauses;
using DemoShop.Domain.Session.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Sessions.Persistence;

public class ShoppingSessionConfiguration : IEntityTypeConfiguration<ShoppingSessionEntity>
{
    public void Configure(EntityTypeBuilder<ShoppingSessionEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseEntityConfiguration.Configure(builder);
        BaseEntityConfiguration.ConfigureAudit(builder);

        builder.ToTable("ShoppingSession");

        builder.HasOne(s => s.User)
            .WithMany(u => u.ShoppingSessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.CartItems)
            .WithOne(c => c.ShoppingSession)
            .HasForeignKey(c => c.ShoppingSessionId);
    }
}
