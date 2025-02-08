#region

using Ardalis.GuardClauses;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using DemoShop.Domain.User.ValueObjects;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

#endregion

namespace DemoShop.Infrastructure.Features.Users;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<UserEntity?> GetUserByKeycloakIdAsync(string value, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var keycloakUserId = KeycloakUserId.Create(value);

        return await context.Query<UserEntity>()
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.KeycloakUserId.Equals(keycloakUserId), cancellationToken);
    }

    public async Task<UserEntity?> CreateUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
        Guard.Against.Null(user, nameof(user));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var entry = await context.Set<UserEntity>().AddAsync(user, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task<UserEntity> UpdateUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
        Guard.Against.Null(user, nameof(user));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var entry = context.Set<UserEntity>().Update(user);
        await context.SaveChangesAsync(cancellationToken);

        await entry.ReloadAsync(cancellationToken);
        return entry.Entity;
    }
}
