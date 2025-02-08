#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace DemoShop.Infrastructure.Common.Persistence;

public static class BaseConfigurations
{
    public static void ConfigureEntity<T>(EntityTypeBuilder<T> builder) where T : class, IEntity
    {
        Guard.Against.Null(builder, nameof(builder));
        builder.HasKey(e => e.Id);
    }

    public static void ConfigureAudit<T>(EntityTypeBuilder<T> builder) where T : class, IAuditable
    {
        Guard.Against.Null(builder, nameof(builder));

        builder.OwnsOne(e => e.Audit, navigationBuilder =>
        {
            navigationBuilder.Property(a => a.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            navigationBuilder.Property(a => a.ModifiedAt)
                .HasColumnName("updated_at")
                .IsRequired();
        });
    }

    public static void ConfigureSoftDelete<T>(EntityTypeBuilder<T> builder) where T : class, ISoftDeletable
    {
        Guard.Against.Null(builder, nameof(builder));

        builder.OwnsOne(e => e.SoftDelete, navigationBuilder =>
        {
            navigationBuilder.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at");

            navigationBuilder.Property(e => e.Deleted)
                .HasColumnName("deleted")
                .IsRequired()
                .HasDefaultValue(false);
        });

        builder.HasQueryFilter(e => !e.SoftDelete.Deleted);
    }
}
