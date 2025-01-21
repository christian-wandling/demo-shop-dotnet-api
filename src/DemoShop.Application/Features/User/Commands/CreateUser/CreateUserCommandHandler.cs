using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.User.Logging;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository repository,
    ILogger<CreateUserCommandHandler> logger
)
    : IRequestHandler<CreateUserCommand, Result<UserEntity>>
{
    public async Task<Result<UserEntity>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        logger.LogUserCreateStarted(request.Email);

        var userResult = UserEntity.Create(
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
            return Result<UserEntity>.Error("Failed to create user");
        }

        logger.LogUserCreated($"{user.Id}");
        return Result<UserEntity>.Success(user);
    }
}
