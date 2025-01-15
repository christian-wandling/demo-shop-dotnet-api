using DemoShop.Domain.Users.Entities;

namespace DemoShop.Domain.Users.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken);
}
