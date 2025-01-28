using System.Linq.Expressions;
using Ardalis.GuardClauses;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.Domain.ShoppingSession.Operations;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DemoShop.Infrastructure.Features.ShoppingSessions;

public class ShoppingSessionRepository(ApplicationDbContext context) : IShoppingSessionRepository
{
    public Task<ShoppingSessionEntity?> GetSessionByIdAsync(int id, CancellationToken cancellationToken) =>
        GetSessionAsync(s => s.Id == id, cancellationToken);

    public Task<ShoppingSessionEntity?> GetSessionByKeycloakIdAsync(string keycloakId, CancellationToken cancellationToken) =>
        GetSessionAsync(s => s.User!.KeycloakUserId.Value == keycloakId, cancellationToken);

    private async Task<ShoppingSessionEntity?> GetSessionAsync(Expression<Func<ShoppingSessionEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(predicate, nameof(predicate));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        return await context.Query<ShoppingSessionEntity>()
            .AsNoTracking()
            .Include(s => s.CartItems)
            .ThenInclude(c => c.Product)
            .ThenInclude(p => p!.Images)
            .FirstOrDefaultAsync(predicate, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<ShoppingSessionEntity?> CreateSessionAsync(ShoppingSessionEntity sessionEntity, CancellationToken cancellationToken)
    {
        Guard.Against.Null(sessionEntity, nameof(sessionEntity));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var createdSession = context.Set<ShoppingSessionEntity>().Add(sessionEntity);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return createdSession.Entity;
    }

    public async Task DeleteAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(sessionId, nameof(sessionId));
        Guard.Against.NegativeOrZero(userId, nameof(userId));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var shoppingSession = await context.Query<ShoppingSessionEntity>()
            .FirstAsync(s => s.Id == sessionId && s.UserId == userId, cancellationToken)
            .ConfigureAwait(false);

        context.Set<ShoppingSessionEntity>().Remove(shoppingSession);
        var result = await context.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        Guard.Against.Null(result, nameof(result));
    }

    public async Task<CartItemEntity?> AddItemAsync(int sessionId, CartItemEntity itemEntity, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(sessionId, nameof(sessionId));
        Guard.Against.Null(itemEntity, nameof(itemEntity));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var createdItem = await context.Set<CartItemEntity>().AddAsync(itemEntity, cancellationToken)
            .ConfigureAwait(false);

        await context.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return createdItem.Entity;
    }

    public async Task<CartItemEntity> UpdateItemAsync(int sessionId, UpdateCartItemQuantity update,
        CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(sessionId, nameof(sessionId));
        Guard.Against.Null(update, nameof(update));

        var cartItem = await context.Query<CartItemEntity>()
            .Include(c => c.Product)
            .ThenInclude(p => p!.Images)
            .FirstAsync(x => x.Id == update.Id && x.ShoppingSessionId == sessionId, cancellationToken)
            .ConfigureAwait(false);

        cartItem.Quantity = update.Quantity;

        await context.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return cartItem;
    }

    public async Task RemoveItemFromSessionAsync(int sessionId, CartItemEntity itemEntity, CancellationToken cancellationToken)
    {
        Guard.Against.Null(itemEntity, nameof(itemEntity));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        context.Set<CartItemEntity>().Remove(itemEntity);

        var result = await context.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        Guard.Against.Null(result, nameof(result));
    }
}
