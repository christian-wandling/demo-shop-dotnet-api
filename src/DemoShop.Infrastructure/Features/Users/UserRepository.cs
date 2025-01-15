using System.Linq.Expressions;
using Ardalis.GuardClauses;
using DemoShop.Domain.Users.Entities;
using DemoShop.Domain.Users.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DemoShop.Infrastructure.Features.Users;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken) =>
        GetUserAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken) =>
        GetUserAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        Guard.Against.Null(user, nameof(user));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var createdUser = await context.Users.AddAsync(user, cancellationToken)
            .ConfigureAwait(false);

        await context.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return createdUser.Entity;
    }

    private async Task<User?> GetUserAsync(Expression<Func<User, bool>> predicate,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(predicate, nameof(predicate));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        return await context.Users
            .AsNoTracking()
            .Include(u => u.Address)
            .FirstOrDefaultAsync(predicate, cancellationToken)
            .ConfigureAwait(false);
    }
}
