#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Product.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace DemoShop.Infrastructure.Features.Products.Persistence;

public class ImageConfiguration : IEntityTypeConfiguration<ImageEntity>
{
    public void Configure(EntityTypeBuilder<ImageEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseConfigurations.ConfigureEntity(builder);
        BaseConfigurations.ConfigureAudit(builder);
        BaseConfigurations.ConfigureSoftDelete(builder);

        builder.ToTable("image");

        builder.Property(i => i.Name)
            .IsRequired();

        builder.Property(i => i.Uri)
            .HasConversion(
                uri => uri.ToString(),
                str => new Uri(str)
            )
            .IsRequired();

        builder.HasIndex(i => i.Uri)
            .IsUnique();

        builder.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId)
            .IsRequired(false);
    }
}
