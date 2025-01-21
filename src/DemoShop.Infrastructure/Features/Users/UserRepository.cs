using System.Linq.Expressions;
using Ardalis.GuardClauses;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DemoShop.Infrastructure.Features.Users;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public Task<UserEntity?> GetUserByIdAsync(int id, CancellationToken cancellationToken) =>
        GetUserAsync(u => u.Id == id, cancellationToken);

    public Task<UserEntity?> GetUserByEmailAsync(string email, CancellationToken cancellationToken) =>
        GetUserAsync(u => u.Email == email, cancellationToken);

    public async Task<UserEntity?> CreateUserAsync(UserEntity userEntity, CancellationToken cancellationToken)
    {
        Guard.Against.Null(userEntity, nameof(userEntity));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var createdUser = await context.Set<UserEntity>().AddAsync(userEntity, cancellationToken)
            .ConfigureAwait(false);

        await context.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return createdUser.Entity;
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
