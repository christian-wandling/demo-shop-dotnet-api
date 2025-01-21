using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.Common.Extensions;
using DemoShop.Application.Features.User.Logging;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository repository,
    ILogger<CreateUserCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<CreateUserCommand> validator
)
    : IRequestHandler<CreateUserCommand, Result<UserEntity>>
{
    public async Task<Result<UserEntity>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UserIdentity, nameof(request.UserIdentity));

        logger.LogUserCreateStarted(request.UserIdentity.Email);

        var validationResult = await validator.ValidateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            logger.LogUserCreateValidationFailed(string.Join(", ", errors));

            return Result<UserEntity>.Invalid(validationResult.Errors.ToValidationErrors());
        }

        var userResult = UserEntity.Create(request.UserIdentity);

        if (!userResult.IsSuccess)
        {
            logger.LogUserCreateValidationFailed(string.Join(", ", userResult.Errors));
            return userResult;
        }

        var user = await repository.CreateUserAsync(userResult.Value, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
        {
            logger.LogUserCreateFailed(request.UserIdentity.Email);
            return Result<UserEntity>.Error("Failed to create user");
        }

        await eventDispatcher.DispatchEventsAsync(user, cancellationToken).ConfigureAwait(false);
        return Result<UserEntity>.Success(user);
    }
}
