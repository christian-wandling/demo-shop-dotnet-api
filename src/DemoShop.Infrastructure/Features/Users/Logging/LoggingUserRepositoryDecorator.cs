using Ardalis.GuardClauses;
using DemoShop.Domain.Users.Entities;
using DemoShop.Domain.Users.Interfaces;
using Microsoft.Extensions.Logging;

namespace DemoShop.Infrastructure.Features.Users.Logging;

public class LoggingUserRepositoryDecorator(
    IUserRepository repository,
    ILogger<LoggingUserRepositoryDecorator> logger
) : IUserRepository
{
    public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
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

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
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

    public async Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            Guard.Against.Null(user, nameof(user));
            Guard.Against.Null(cancellationToken, nameof(cancellationToken));

            logger.LogCreateUserStarted(user.Email);
            var createdUser = await repository.CreateUserAsync(user, cancellationToken).ConfigureAwait(false);
            logger.LogCreateUserSuccess($"{createdUser!.Id}");

            return user;
        }
        catch (Exception ex)
        {
            logger.LogCreateUserFailed(user?.Email ?? string.Empty, ex);
            throw;
        }
    }
}
