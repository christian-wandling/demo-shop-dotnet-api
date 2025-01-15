using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.Users.Logging;
using DemoShop.Domain.Users.Entities;
using DemoShop.Domain.Users.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository repository,
    ILogger<CreateUserCommandHandler> logger
)
    : IRequestHandler<CreateUserCommand, Result<User>?>
{
    public async Task<Result<User>?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        logger.LogUserCreateStarted(request.Email);

        var userResult = User.Create(
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
            return Result<User>.Error("Failed to create user");
        }

        logger.LogUserCreated($"{user.Id}");
        return Result<User>.Success(user);
    }
}
