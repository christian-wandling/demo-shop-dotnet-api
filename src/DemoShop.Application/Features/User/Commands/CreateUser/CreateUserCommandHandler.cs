#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IMapper mapper,
    IUserRepository repository,
    ILogger<CreateUserCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<CreateUserCommand> validator,
    IValidationService validationService
)
    : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UserIdentity, nameof(request.UserIdentity));
        Guard.Against.Null(cancellationToken, nameof(CancellationToken));

        var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);

        if (!validationResult.IsSuccess)
            return validationResult.Map();

        try
        {
            var unsavedResult = UserEntity.Create(request.UserIdentity);

            if (!unsavedResult.IsSuccess)
                return unsavedResult.Map();

            var savedResult = await SaveChanges(unsavedResult.Value, cancellationToken);

            return savedResult.IsSuccess
                ? Result.Success(mapper.Map<UserResponse>(savedResult.Value))
                : savedResult.Map();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogOperationFailed("Create user", "KeycloakUserId", request.UserIdentity.KeycloakUserId, ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<Result<UserEntity>> SaveChanges(UserEntity unsavedUser, CancellationToken cancellationToken)
    {
        var savedUser = await repository.CreateUserAsync(unsavedUser, cancellationToken);

        if (savedUser is null)
            return Result.Error("Failed to create user");

        await eventDispatcher.DispatchEventsAsync(unsavedUser, cancellationToken);

        return Result.Success(savedUser);
    }
}
