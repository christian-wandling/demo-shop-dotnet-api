using System.Linq.Expressions;
using Ardalis.GuardClauses;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using DemoShop.Domain.User.ValueObjects;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DemoShop.Infrastructure.Features.Users;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public Task<UserEntity?> GetUserByIdAsync(int id, CancellationToken cancellationToken) =>
        GetUserAsync(u => u.Id == id, cancellationToken);

    public Task<UserEntity?> GetUserByKeycloakIdAsync(string keycloakId, CancellationToken cancellationToken) =>
        GetUserAsync(u => u.KeycloakUserId.Equals(KeycloakUserId.Create(keycloakId)), cancellationToken);

    public async Task<UserEntity?> CreateUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
        Guard.Against.Null(user, nameof(user));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var createdUser = await context.Set<UserEntity>().AddAsync(user, cancellationToken)
            .ConfigureAwait(false);

        await context.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return createdUser.Entity;
    }

    public async Task UpdateUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
        Guard.Against.Null(user, nameof(user));

        var updatedUser = context.Set<UserEntity>().Update(user);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        Guard.Against.Null(updatedUser, nameof(updatedUser));
    }

    private async Task<UserEntity?> GetUserAsync(Expression<Func<UserEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(predicate, nameof(predicate));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        return await context.Query<UserEntity>()
            .Include(u => u.Address)
            .FirstOrDefaultAsync(predicate, cancellationToken)
            .ConfigureAwait(false);
    }
}
