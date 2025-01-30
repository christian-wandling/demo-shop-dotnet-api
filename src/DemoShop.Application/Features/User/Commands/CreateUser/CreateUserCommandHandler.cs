using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Common.Extensions;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IMapper mapper,
    IUserRepository repository,
    ILogger<CreateUserCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<CreateUserCommand> validator
)
    : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UserIdentity, nameof(request.UserIdentity));

        var validationResult = await validator.ValidateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            logger.LogValidationFailed("Create User", string.Join(", ", errors));

            return Result.Invalid(validationResult.Errors.ToValidationErrors());
        }

        var createdUser = UserEntity.Create(request.UserIdentity);
        var user = await repository.CreateUserAsync(createdUser, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
        {
            logger.LogOperationFailed("Create User", "keycloakId", request.UserIdentity.KeycloakUserId, null);
            return Result.Error("Failed to create user");
        }

        await eventDispatcher.DispatchEventsAsync(user, cancellationToken).ConfigureAwait(false);
        return Result.Success(mapper.Map<UserResponse>(user));
    }
}
