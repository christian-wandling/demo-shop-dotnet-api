using Ardalis.GuardClauses;
using DemoShop.Domain.Users.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Users.Persistence;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseEntityConfiguration.Configure(builder);

        builder.HasOne(a => a.User)
            .WithOne(u => u.Address)
            .HasForeignKey<Address>(a => a.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

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
