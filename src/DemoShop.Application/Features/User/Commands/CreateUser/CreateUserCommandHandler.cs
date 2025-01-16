using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.User.Logging;
using DemoShop.Domain.User.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository repository,
    ILogger<CreateUserCommandHandler> logger
)
    : IRequestHandler<CreateUserCommand, Result<Domain.User.Entities.UserEntity>?>
{
    public async Task<Result<Domain.User.Entities.UserEntity>?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        logger.LogUserCreateStarted(request.Email);

        var userResult = Domain.User.Entities.UserEntity.Create(
            request.KeycloakUserId,
            request.Email,
            request.Firstname,
            request.Lastname
        );

        if (!userResult.IsSuccess)
        {
            logger.LogUserCreateValidationFailed(string.Join(", ", userResult.Errors));
            return userResult;
        }

        var user = await repository.CreateUserAsync(userResult.Value, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
        {
            logger.LogUserCreateFailed(request.Email);
            return Result<Domain.User.Entities.UserEntity>.Error("Failed to create user");
        }

        logger.LogUserCreated($"{user.Id}");
        return Result<Domain.User.Entities.UserEntity>.Success(user);
    }
}
