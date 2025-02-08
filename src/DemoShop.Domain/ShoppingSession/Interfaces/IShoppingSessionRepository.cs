#region

using DemoShop.Domain.ShoppingSession.Entities;

#endregion

namespace DemoShop.Domain.ShoppingSession.Interfaces;

public interface IShoppingSessionRepository
{
    Task<ShoppingSessionEntity?> GetSessionByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<ShoppingSessionEntity?> CreateSessionAsync(ShoppingSessionEntity session, CancellationToken cancellationToken);
    Task<bool> DeleteSessionAsync(ShoppingSessionEntity session, CancellationToken cancellationToken);
    Task<ShoppingSessionEntity> UpdateSessionAsync(ShoppingSessionEntity session, CancellationToken cancellationToken);
}
