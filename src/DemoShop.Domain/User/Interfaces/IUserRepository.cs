namespace DemoShop.Domain.User.Interfaces;

public interface IUserRepository
{
    Task<Entities.UserEntity?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<Entities.UserEntity?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Entities.UserEntity?> CreateUserAsync(Entities.UserEntity userEntity, CancellationToken cancellationToken);
}
