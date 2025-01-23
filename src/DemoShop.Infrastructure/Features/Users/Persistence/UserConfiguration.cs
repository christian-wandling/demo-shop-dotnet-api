using Ardalis.GuardClauses;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.ValueObjects;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoShop.Infrastructure.Features.Users.Persistence;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        Guard.Against.Null(builder, nameof(builder));

        BaseConfigurations.ConfigureEntity(builder);
        BaseConfigurations.ConfigureAudit(builder);
        BaseConfigurations.ConfigureSoftDelete(builder);

        builder.ToTable("User");

        builder.Property(u => u.KeycloakUserId)
            .IsRequired()
            .HasConversion(
                keycloakUserId => keycloakUserId.Value.ToString(),
                dbKeycloakUserId => KeycloakUserId.Create(Guid.Parse(dbKeycloakUserId))
            );

        builder.HasIndex(u => u.KeycloakUserId)
            .IsUnique();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasConversion(
                email => email.Value,
                dbEmail => EmailAddress.Create(dbEmail)
            );

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Phone)
            .IsRequired(false)
            .HasConversion(
                phone => phone == null ? null : phone.Value,
                dbPhone => PhoneNumber.Create(dbPhone)
            );

        builder.OwnsOne(u => u.PersonName, navigationBuilder =>
        {
            navigationBuilder.WithOwner().HasForeignKey("id");

            navigationBuilder.Property(n => n.Firstname)
                .HasColumnName("firstname");

            navigationBuilder.Property(n => n.Lastname)
                .HasColumnName("lastname");
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
