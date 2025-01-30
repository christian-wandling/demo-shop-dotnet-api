using System.Linq.Expressions;
using Ardalis.GuardClauses;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DemoShop.Infrastructure.Features.ShoppingSessions;

public class ShoppingSessionRepository(ApplicationDbContext context) : IShoppingSessionRepository
{
    public async Task<ShoppingSessionEntity?> GetSessionByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(userId, nameof(userId));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        return await context.Query<ShoppingSessionEntity>()
            .Include(s => s.CartItems)
            .ThenInclude(c => c.Product)
            .ThenInclude(p => p!.Images)
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<ShoppingSessionEntity?> CreateSessionAsync(ShoppingSessionEntity session,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(session, nameof(session));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var createdSession = await context.Set<ShoppingSessionEntity>()
            .AddAsync(session, cancellationToken)
            .ConfigureAwait(false);

        await context.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return createdSession.Entity;
    }

    public async Task UpdateSessionAsync(
        ShoppingSessionEntity session,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(session, nameof(session));

        var updatedSession = context.Set<ShoppingSessionEntity>()
            .Update(session);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        Guard.Against.Null(updatedSession, nameof(updatedSession));
    }
}
