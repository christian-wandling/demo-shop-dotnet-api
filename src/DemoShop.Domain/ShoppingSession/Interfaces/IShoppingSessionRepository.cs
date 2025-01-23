using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Operations;

namespace DemoShop.Domain.ShoppingSession.Interfaces;

public interface IShoppingSessionRepository
{
    Task<ShoppingSessionEntity?> GetSessionByIdAsync(int id, CancellationToken cancellationToken);
    Task<ShoppingSessionEntity?> GetSessionByEmailAsync(string email, CancellationToken cancellationToken);
    Task<ShoppingSessionEntity?> CreateSessionAsync(ShoppingSessionEntity sessionEntity, CancellationToken cancellationToken);
    Task DeleteAsync(int sessionId, int userId, CancellationToken cancellationToken);
    Task<CartItemEntity?> AddItemAsync(int sessionId, CartItemEntity itemEntity, CancellationToken cancellationToken);
    Task<CartItemEntity> UpdateItemAsync(int sessionId, UpdateCartItemQuantity update, CancellationToken cancellationToken);
    Task RemoveItemFromSessionAsync(int sessionId, CartItemEntity itemEntity, CancellationToken cancellationToken);
}
