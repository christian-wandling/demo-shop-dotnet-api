using Ardalis.GuardClauses;
using DemoShop.Domain.Product.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Products.Persistence;

public class ImageConfiguration : IEntityTypeConfiguration<ImageEntity>
{
    public void Configure(EntityTypeBuilder<ImageEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseEntityConfiguration.Configure(builder);
        BaseEntityConfiguration.ConfigureAudit(builder);
        BaseEntityConfiguration.ConfigureSoftDelete(builder);

        builder.ToTable("Image");

        builder.Property(i => i.Name)
            .IsRequired();

        builder.Property(i => i.Uri)
            .IsRequired();

        builder.HasIndex(i => i.Uri)
            .IsUnique();

        builder.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId)
            .IsRequired(false);
    }
}
