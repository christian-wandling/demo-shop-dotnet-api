using Ardalis.GuardClauses;
using DemoShop.Domain.User.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Users.Persistence;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseEntityConfiguration.ConfigureWithSoftDelete(builder);

        builder.ToTable("User");

        builder.Property(u => u.KeycloakUserId)
            .IsRequired()
            .HasConversion(
                guid => guid.ToString(),
                str => Guid.Parse(str)
            );

        builder.HasIndex(u => u.KeycloakUserId)
            .IsUnique();

        builder.Property(u => u.Email)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Firstname)
            .IsRequired();

        builder.Property(u => u.Lastname)
            .IsRequired();

        builder.Property(u => u.Phone);

        builder.HasOne(u => u.Address)
            .WithOne(a => a.User)
            .HasForeignKey<AddressEntity>(a => a.UserId)
            .IsRequired();

        builder.HasMany(u => u.ShoppingSessions)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId);

        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(s => s.UserId);
    }
}
