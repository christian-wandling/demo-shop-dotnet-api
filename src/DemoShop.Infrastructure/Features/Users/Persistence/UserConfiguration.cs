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

        BaseEntityConfiguration.Configure(builder);
        BaseEntityConfiguration.ConfigureAudit(builder);
        BaseEntityConfiguration.ConfigureSoftDelete(builder);

        builder.ToTable("User");

        builder.OwnsOne(u => u.KeycloakUserId, keycloakUserId =>
        {
            keycloakUserId.Property(k => k.Value)
                .IsRequired()
                .HasConversion(
                    guid => guid.ToString(),
                    str => Guid.Parse(str)
                );

            keycloakUserId.HasIndex(k => k.Value)
                .IsUnique();
        });

        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired();

            email.HasIndex(e => e.Value)
                .IsUnique();
        });

        builder.OwnsOne(u => u.PersonName, name =>
        {
            name.Property(n => n.Firstname)
                .HasColumnName("firstname")
                .IsRequired();
            name.Property(n => n.Lastname)
                .HasColumnName("lastname")
                .IsRequired();
        });

        builder.OwnsOne(u => u.Phone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("phone")
                .IsRequired();
        });

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
