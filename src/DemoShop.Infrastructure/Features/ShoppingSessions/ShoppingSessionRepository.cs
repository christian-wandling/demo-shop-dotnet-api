#region

using Ardalis.GuardClauses;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

#endregion

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
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task<ShoppingSessionEntity?> CreateSessionAsync(ShoppingSessionEntity session,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(session, nameof(session));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var createdSession = await context.Set<ShoppingSessionEntity>()
            .AddAsync(session, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return createdSession.Entity;
    }

    public async Task<ShoppingSessionEntity> UpdateSessionAsync(
        ShoppingSessionEntity session,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(session, nameof(session));

        var entry = context.Set<ShoppingSessionEntity>().Update(session);
        await context.SaveChangesAsync(cancellationToken);

        await entry.ReloadAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task<bool> DeleteSessionAsync(
        ShoppingSessionEntity session,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(session, nameof(session));

        context.Set<ShoppingSessionEntity>().Remove(session);
        var affected = await context.SaveChangesAsync(cancellationToken);

        return affected > 0;
    }
}
