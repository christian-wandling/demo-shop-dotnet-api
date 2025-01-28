using Ardalis.GuardClauses;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.ValueObjects;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Products.Persistence;

public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseConfigurations.ConfigureEntity(builder);
        BaseConfigurations.ConfigureAudit(builder);
        BaseConfigurations.ConfigureSoftDelete(builder);

        builder.ToTable("product");

        builder.Property(p => p.Name)
            .IsRequired();

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.Price)
            .IsRequired()
            .HasConversion(
                price => price.Value,
                dbPrice => Price.Create(dbPrice)
            );

        builder.HasMany(p => p.Categories)
            .WithMany(c => c.Products)
            .UsingEntity("category_product",
                l => l.HasOne(typeof(CategoryEntity)).WithMany().HasForeignKey("category_id"),
                r => r.HasOne(typeof(ProductEntity)).WithMany().HasForeignKey("product_id"));

        builder.HasMany(p => p.Images)
            .WithOne(c => c.Product)
            .HasForeignKey(c => c.ProductId);

        builder.HasMany(p => p.CartItems)
            .WithOne(c => c.Product)
            .HasForeignKey(c => c.ProductId);
    }
}
