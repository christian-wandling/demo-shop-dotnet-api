#region

using Ardalis.GuardClauses;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace DemoShop.Infrastructure.Features.ShoppingSessions.Persistence;

public class ShoppingSessionConfiguration : IEntityTypeConfiguration<ShoppingSessionEntity>
{
    public void Configure(EntityTypeBuilder<ShoppingSessionEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseConfigurations.ConfigureEntity(builder);
        BaseConfigurations.ConfigureAudit(builder);

        builder.ToTable("shopping_session");

        builder.HasOne(s => s.User)
            .WithMany(u => u.ShoppingSessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(a => a.User != null && !a.User.SoftDelete.Deleted);

        builder.HasMany(s => s.CartItems)
            .WithOne(c => c.ShoppingSession)
            .HasForeignKey(c => c.ShoppingSessionId);
    }
}
