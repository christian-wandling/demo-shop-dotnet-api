using DemoShop.Domain.ShoppingSession.Entities;

namespace DemoShop.Domain.ShoppingSession.Interfaces;

public interface IShoppingSessionRepository
{
    Task<ShoppingSessionEntity?> GetSessionByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<ShoppingSessionEntity?> CreateSessionAsync(ShoppingSessionEntity session, CancellationToken cancellationToken);
    Task UpdateSessionAsync(ShoppingSessionEntity session, CancellationToken cancellationToken);
}
