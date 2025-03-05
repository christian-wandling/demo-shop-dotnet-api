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
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IMapper mapper,
    IUserRepository repository,
    ILogger logger,
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

        try
        {
            LogCommandStarted(logger, request.UserIdentity.KeycloakUserId);

            var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);
            if (!validationResult.IsSuccess)
            {
                LogCommandError(logger, request.UserIdentity.KeycloakUserId);
                return validationResult.Map();
            }

            var unsavedResult = UserEntity.Create(request.UserIdentity);
            if (!unsavedResult.IsSuccess)
            {
                LogCommandError(logger, request.UserIdentity.KeycloakUserId);
                return unsavedResult.Map();
            }

            var savedResult = await SaveChanges(unsavedResult.Value, cancellationToken);
            if (!savedResult.IsSuccess)
            {
                LogCommandError(logger, request.UserIdentity.KeycloakUserId);
                return savedResult.Map();
            }

            LogCommandSuccess(logger, savedResult.Value.Id, savedResult.Value.KeycloakUserId.Value);
            return Result.Success(mapper.Map<UserResponse>(savedResult.Value));
        }
        catch (InvalidOperationException ex)
        {
            LogInvalidOperationException(logger, ex.Message, ex);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            LogDatabaseException(logger, ex.Message, ex);
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

    private static void LogCommandStarted(ILogger logger, string keycloakUserId) =>
        logger.ForContext("EventId", LoggerEventIds.CreateUserCommandStarted)
            .Information("Starting to create user with KeycloakUserId {KeycloakUserId}", keycloakUserId);

    private static void LogCommandSuccess(ILogger logger, int userId, string keycloakUserId) =>
        logger.ForContext("EventId", LoggerEventIds.CreateUserCommandSuccess)
            .Information("Successfully created user with Id {UserId} for KeycloakUserId {KeycloakUserId}",
                userId, keycloakUserId);

    private static void LogCommandError(ILogger logger, string keycloakUserId) =>
        logger.ForContext("EventId", LoggerEventIds.CreateUserCommandError)
            .Information("Error creating user with KeycloakUserId {KeycloakUserId}", keycloakUserId);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) =>
        logger.Error(ex, "Database error occurred while creating user. Error: {ErrorMessage} {@EventId}",
            errorMessage, LoggerEventIds.CreateUserDatabaseException);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) =>
        logger.Error(ex, "Invalid operation while creating user. Error: {ErrorMessage} {@EventId}",
            errorMessage, LoggerEventIds.CreateUserDomainException);
}
