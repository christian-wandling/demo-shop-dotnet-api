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

    public async Task UpdateUserPhoneAsync(UserEntity user, CancellationToken cancellationToken)
    {
        Guard.Against.Null(user, nameof(user));
        Guard.Against.Null(user.Phone, nameof(user.Phone));

        var rowsAffected = await context.Set<UserEntity>()
            .Where(u => u.Email.Equals(user.Email))
            .ExecuteUpdateAsync(s =>
                s.SetProperty(u => u.Phone, user.Phone), cancellationToken)
            .ConfigureAwait(false);

        if (rowsAffected == 0)
        {
            throw new NotFoundException(nameof(UserEntity), user.Email.Value);
        }
    }

    private async Task<UserEntity?> GetUserAsync(Expression<Func<UserEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(predicate, nameof(predicate));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        return await context.Query<UserEntity>()
            .AsNoTracking()
            .Include(u => u.Address)
            .FirstOrDefaultAsync(predicate, cancellationToken)
            .ConfigureAwait(false);
    }
}
