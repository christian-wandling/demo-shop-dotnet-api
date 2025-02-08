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
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.User.Commands.UpdateUserAddress;

public sealed class UpdateUserAddressCommandHandler(
    IMapper mapper,
    IUserIdentityAccessor identity,
    IUserRepository repository,
    ILogger<UpdateUserAddressCommandHandler> logger,
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

        var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);

        if (!validationResult.IsSuccess)
            return validationResult.Map();

        var identityResult = identity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
            return identityResult.Map<IUserIdentity, AddressResponse>(null);

        try
        {
            var user = await repository.GetUserByKeycloakIdAsync(
                identityResult.Value.KeycloakUserId,
                cancellationToken
            );

            if (user is null)
                return Result.NotFound("User not found");

            var unsavedResult = UpdateUserAddress(user, request.UpdateUserAddress);

            if (!unsavedResult.IsSuccess)
                return unsavedResult.Map();

            var savedResult = await SaveChanges(user, cancellationToken);

            return savedResult.IsSuccess
                ? Result.Success(mapper.Map<AddressResponse>(savedResult.Value.Address))
                : savedResult.Map();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogOperationFailed("Update user address", "KeycloakUserId", identityResult.Value.KeycloakUserId, ex);
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
}
