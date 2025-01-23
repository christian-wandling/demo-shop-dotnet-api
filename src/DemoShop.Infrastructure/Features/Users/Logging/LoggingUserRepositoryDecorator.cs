using Ardalis.GuardClauses;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using Microsoft.Extensions.Logging;

namespace DemoShop.Infrastructure.Features.Users.Logging;

public class LoggingUserRepositoryDecorator(
    IUserRepository repository,
    ILogger<LoggingUserRepositoryDecorator> logger
) : IUserRepository
{
    public async Task<UserEntity?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            Guard.Against.NegativeOrZero(id, nameof(id));
            Guard.Against.Null(cancellationToken, nameof(cancellationToken));

            logger.LogGetUserByIdStarted($"{id}");
            var user = await repository.GetUserByIdAsync(id, cancellationToken).ConfigureAwait(false);
            logger.LogGetUserByIdSuccess($"{id}");

            return user;
        }
        catch (Exception ex)
        {
            logger.LogGetUserByIdFailed($"{id}", ex);
            throw;
        }
    }

    public async Task<UserEntity?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(email, nameof(email));
            Guard.Against.Null(cancellationToken, nameof(cancellationToken));

            logger.LogGetUserByEmailStarted(email);
            var user = await repository.GetUserByEmailAsync(email, cancellationToken).ConfigureAwait(false);
            logger.LogGetUserByEmailSuccess(email);

            return user;
        }
        catch (Exception ex)
        {
            logger.LogGetUserByEmailFailed(email, ex);
            throw;
        }
    }

    public async Task<UserEntity?> CreateUserAsync(UserEntity userEntity, CancellationToken cancellationToken)
    {
        try
        {
            Guard.Against.Null(userEntity, nameof(userEntity));
            Guard.Against.Null(cancellationToken, nameof(cancellationToken));

            logger.LogCreateUserStarted(userEntity.Email.Value);
            var createdUser = await repository.CreateUserAsync(userEntity, cancellationToken).ConfigureAwait(false);
            logger.LogCreateUserSuccess($"{createdUser!.Id}");

            return userEntity;
        }
        catch (Exception ex)
        {
            logger.LogCreateUserFailed(userEntity?.Email.Value ?? string.Empty, ex);
            throw;
        }
    }
}
