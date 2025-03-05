#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.DTOs;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Commands.UpdateUserAddress;

public sealed class UpdateUserAddressCommandHandler(
    IMapper mapper,
    IUserIdentityAccessor identity,
    IUserRepository repository,
    ILogger logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<UpdateUserAddressCommand> validator,
    IValidationService validationService
)
    : IRequestHandler<UpdateUserAddressCommand, Result<AddressResponse>>
{
    public async Task<Result<AddressResponse>> Handle(UpdateUserAddressCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UpdateUserAddress, nameof(request.UpdateUserAddress));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var identityResult = identity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
            return identityResult.Map<IUserIdentity, AddressResponse>(null);

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

            var unsavedResult = UpdateUserAddress(user, request.UpdateUserAddress);
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
            return Result.Success(mapper.Map<AddressResponse>(savedResult.Value.Address));
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

    private static Result UpdateUserAddress(UserEntity user, IUpdateUserAddressRequest updateAddress)
    {
        if (user.Address == null)
        {
            var createAddress = new CreateAddressDto
            {
                UserId = user.Id,
                Street = updateAddress.Street,
                Apartment = updateAddress.Apartment,
                City = updateAddress.City,
                Country = updateAddress.Country,
                Zip = updateAddress.Zip,
                Region = updateAddress.Region
            };

            return user.SetInitialAddress(createAddress);
        }

        var address = new UpdateAddressDto
        {
            UserId = user.Id,
            Street = updateAddress.Street,
            Apartment = updateAddress.Apartment,
            City = updateAddress.City,
            Country = updateAddress.Country,
            Zip = updateAddress.Zip,
            Region = updateAddress.Region
        };

        return user.UpdateAddress(address);
    }

    private async Task<Result<UserEntity>> SaveChanges(UserEntity unsavedUser, CancellationToken cancellationToken)
    {
        var savedUser = await repository.UpdateUserAsync(unsavedUser, cancellationToken);

        await eventDispatcher.DispatchEventsAsync(unsavedUser, cancellationToken);
        return Result.Success(savedUser);
    }

    private static void LogCommandStarted(ILogger logger, string keycloakUserId) => logger
        .ForContext("EventId", LoggerEventId.UpdateUserAddressCommandStarted)
        .Debug("Starting to update address for user with KeycloakUserId {KeycloakUserId}", keycloakUserId);

    private static void LogCommandSuccess(ILogger logger, int userId) => logger
        .ForContext("EventId", LoggerEventId.UpdateUserAddressCommandSuccess)
        .Information("Successfully updated address for user with Id {UserId}", userId);

    private static void LogCommandError(ILogger logger, string keycloakUserId) => logger
        .ForContext("EventId", LoggerEventId.UpdateUserAddressCommandError)
        .Error("Error updating address for user with KeycloakUserId {KeycloakUserId}", keycloakUserId);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.UpdateUserDatabaseException)
        .Error(ex, "Database error occurred while updating user address. Error: {ErrorMessage}", errorMessage);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.UpdateUserDomainException)
        .Error(ex, "Invalid operation while updating user address. Error: {ErrorMessage}", errorMessage);
}
