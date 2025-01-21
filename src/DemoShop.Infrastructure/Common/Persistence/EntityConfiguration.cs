using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Common.Persistence;

public static class BaseEntityConfiguration
{
    public static void Configure<T>(EntityTypeBuilder<T> builder) where T : class, IEntity
    {
        Guard.Against.Null(builder, nameof(builder));

        builder.HasKey(e => e.Id);
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        builder.Property(e => e.ModifiedAt).IsRequired();
    }

    public static void ConfigureWithSoftDelete<T>(EntityTypeBuilder<T> builder) where T : class, IEntity, ISoftDeletable
    {
        Guard.Against.Null(builder, nameof(builder));

        Configure(builder);
        builder.Property(e => e.Deleted);
        builder.Property(e => e.DeletedAt)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
