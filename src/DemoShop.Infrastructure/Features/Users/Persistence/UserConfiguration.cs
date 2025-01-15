using Ardalis.GuardClauses;
using DemoShop.Domain.Users.Entities;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Users.Persistence;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseEntityConfiguration.ConfigureWithSoftDelete(builder);

        builder.Property(u => u.KeycloakUserId)
            .IsRequired();

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
            .HasForeignKey<Address>(a => a.UserId)
            .IsRequired(false);

        builder.HasMany(u => u.ShoppingSessions)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId);

        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(s => s.UserId);
    }
}
