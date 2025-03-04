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

namespace DemoShop.Application.Features.User.Commands.UpdateUserPhone;

public sealed class UpdateUserPhoneCommandHandler(
    IMapper mapper,
    IUserIdentityAccessor identity,
    IUserRepository repository,
    ILogger logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<UpdateUserPhoneCommand> validator,
    IValidationService validationService
)
    : IRequestHandler<UpdateUserPhoneCommand, Result<UserPhoneResponse>>
{
    public async Task<Result<UserPhoneResponse>> Handle(UpdateUserPhoneCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UpdateUser, nameof(request.UpdateUser));
        Guard.Against.Null(cancellationToken, nameof(CancellationToken));

        var identityResult = identity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
            return identityResult.Map<IUserIdentity, UserPhoneResponse>(null);

        try
        {
            LogCommandStarted(logger, identityResult.Value.KeycloakUserId);

            var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);
            if (!validationResult.IsSuccess)
            {
                LogCommandError(logger, identityResult.Value.KeycloakUserId);
                return validationResult.Map();
            }

            var user = await repository.GetUserByKeycloakIdAsync(
                identityResult.Value.KeycloakUserId,
                cancellationToken);
            if (user is null)
            {
                LogCommandError(logger, identityResult.Value.KeycloakUserId);
                return Result.NotFound("User not found");
            }

            var unsavedResult = user.UpdatePhone(request.UpdateUser.Phone);
            if (!unsavedResult.IsSuccess)
            {
                LogCommandError(logger, identityResult.Value.KeycloakUserId);
                return unsavedResult.Map();
            }

            var savedResult = await SaveChanges(user, cancellationToken);
            if (!savedResult.IsSuccess)
            {
                LogCommandError(logger, identityResult.Value.KeycloakUserId);
                return savedResult.Map();
            }

            LogCommandSuccess(logger, savedResult.Value.Id);
            return Result.Success(mapper.Map<UserPhoneResponse>(user));
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
        var savedUser = await repository.UpdateUserAsync(unsavedUser, cancellationToken);

        await eventDispatcher.DispatchEventsAsync(unsavedUser, cancellationToken);
        return Result.Success(savedUser);
    }

    private static void LogCommandStarted(ILogger logger, string keycloakUserId) =>
        logger.ForContext("EventId", LoggerEventIds.UpdateUserPhoneCommandStarted)
            .Information("Starting to update phone for user with KeycloakUserId {KeycloakUserId}",
                keycloakUserId);

    private static void LogCommandSuccess(ILogger logger, int userId) =>
        logger.ForContext("EventId", LoggerEventIds.UpdateUserPhoneCommandSuccess)
            .Information("Successfully updated phone for user with Id {UserId}", userId);

    private static void LogCommandError(ILogger logger, string keycloakUserId) =>
        logger.ForContext("EventId", LoggerEventIds.UpdateUserPhoneCommandError)
            .Information("Error updating phone for user with KeycloakUserId {KeycloakUserId}", keycloakUserId);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) =>
        logger.Error(ex, "Database error occurred while updating user phone. Error: {ErrorMessage} {@EventId}",
            errorMessage, LoggerEventIds.UpdateUserDatabaseException);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) =>
        logger.Error(ex, "Invalid operation while updating user phone. Error: {ErrorMessage} {@EventId}",
            errorMessage, LoggerEventIds.UpdateUserDomainException);
}
