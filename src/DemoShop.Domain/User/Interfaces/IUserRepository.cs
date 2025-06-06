#region

using DemoShop.Domain.User.Entities;

#endregion

namespace DemoShop.Domain.User.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetUserByKeycloakIdAsync(string keycloakUserIdValue, CancellationToken cancellationToken);
    Task<UserEntity?> GetUserByKeycloakIdAsync(string keycloakUserIdValue, bool trackChanges, CancellationToken cancellationToken);
    Task<UserEntity?> CreateUserAsync(UserEntity user, CancellationToken cancellationToken);
    Task<UserEntity> UpdateUserAsync(UserEntity user, CancellationToken cancellationToken);
}
