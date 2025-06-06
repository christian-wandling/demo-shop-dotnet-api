#region

using Ardalis.GuardClauses;
using DemoShop.Domain.User.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace DemoShop.Infrastructure.Features.Users.Persistence;

public class AddressConfiguration : IEntityTypeConfiguration<AddressEntity>
{
    public void Configure(EntityTypeBuilder<AddressEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseConfigurations.ConfigureEntity(builder);
        BaseConfigurations.ConfigureAudit(builder);

        builder.ToTable("address");

        builder.HasOne(a => a.User)
            .WithOne(u => u.Address)
            .HasForeignKey<AddressEntity>(a => a.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(a => !a.User.SoftDelete.Deleted);

        builder.HasIndex(a => a.UserId)
            .IsUnique();

        builder.Property(a => a.Street)
            .IsRequired();

        builder.Property(a => a.Apartment)
            .IsRequired();

        builder.Property(a => a.City)
            .IsRequired();

        builder.Property(a => a.Zip)
            .IsRequired();

        builder.Property(a => a.Region);

        builder.Property(a => a.Country)
            .IsRequired();
    }
}
