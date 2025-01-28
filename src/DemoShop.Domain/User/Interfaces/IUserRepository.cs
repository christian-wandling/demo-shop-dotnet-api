using DemoShop.Domain.User.Entities;

namespace DemoShop.Domain.User.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<UserEntity?> GetUserByKeycloakIdAsync(string keycloakId, CancellationToken cancellationToken);
    Task<UserEntity?> CreateUserAsync(UserEntity user, CancellationToken cancellationToken);
    Task UpdateUserPhoneAsync(UserEntity user, CancellationToken cancellationToken);
    Task UpdateUserAddressAsync(UserEntity user, CancellationToken cancellationToken);
}
