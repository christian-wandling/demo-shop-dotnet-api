using Ardalis.GuardClauses;
using DemoShop.Domain.Product.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Products.Persistence;

public class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseEntityConfiguration.Configure(builder);
        BaseEntityConfiguration.ConfigureAudit(builder);
        BaseEntityConfiguration.ConfigureSoftDelete(builder);

        builder.ToTable("Category");

        builder.Property(c => c.Name)
            .IsRequired();

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.HasMany(c => c.Products)
            .WithMany(p => p.Categories);
    }
}
