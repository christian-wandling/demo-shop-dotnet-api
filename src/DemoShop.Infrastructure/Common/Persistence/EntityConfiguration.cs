using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Common.Persistence;

public static class BaseEntityConfiguration
{
    public static void Configure<T>(EntityTypeBuilder<T> builder) where T : class, IEntity
    {
        Guard.Against.Null(builder, nameof(builder));

        builder.HasKey(e => e.Id);
    }

    public static void ConfigureAudit<T>(EntityTypeBuilder<T> builder) where T : class, IAuditable
    {
        Guard.Against.Null(builder, nameof(builder));

        builder.OwnsOne(e => e.Audit, audit =>
        {
            audit.Property(a => a.CreatedAt)
                .HasColumnName("createdAt")
                .IsRequired();

            audit.Property(a => a.ModifiedAt)
                .HasColumnName("updatedAt")
                .IsRequired();
        });
    }

    public static void ConfigureSoftDelete<T>(EntityTypeBuilder<T> builder) where T : class, ISoftDeletable
    {
        Guard.Against.Null(builder, nameof(builder));

        builder.OwnsOne(e => e.SoftDeleteAudit, audit =>
        {
            audit.Property(s => s.DeletedAt)
                .HasColumnName("deletedAt")
                .IsRequired();

            audit.Property(s => s.IsDeleted)
                .HasColumnName("deleted")
                .IsRequired()
                .HasDefaultValue(false);
        });
    }
}
