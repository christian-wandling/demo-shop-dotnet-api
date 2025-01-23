using Ardalis.GuardClauses;
using DemoShop.Domain.Product.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Products.Persistence;

public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseEntityConfiguration.Configure(builder);
        BaseEntityConfiguration.ConfigureAudit(builder);
        BaseEntityConfiguration.ConfigureSoftDelete(builder);

        builder.ToTable("Product");

        builder.Property(p => p.Name)
            .IsRequired();

        builder.HasMany(p => p.Categories)
            .WithMany(c => c.Products);

        builder.HasMany(p => p.Images)
            .WithOne(c => c.Product)
            .HasForeignKey(c => c.ProductId);

        builder.HasMany(p => p.CartItems)
            .WithOne(c => c.Product)
            .HasForeignKey(c => c.ProductId);

        builder.Property(p => p.Price)
            .IsRequired();

        builder.Property(p => p.Description)
            .IsRequired();
    }
}
