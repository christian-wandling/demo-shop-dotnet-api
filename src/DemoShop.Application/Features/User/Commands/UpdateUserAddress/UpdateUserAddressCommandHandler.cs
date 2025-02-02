using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Common.Extensions;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.DTOs;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Commands.UpdateUserAddress;

public sealed class UpdateUserAddressCommandHandler(
    IMapper mapper,
    IUserIdentityAccessor identity,
    IUserRepository repository,
    ILogger<UpdateUserAddressCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<UpdateUserAddressCommand> validator
)
    : IRequestHandler<UpdateUserAddressCommand, Result<AddressResponse?>>
{
    public async Task<Result<AddressResponse?>> Handle(UpdateUserAddressCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UpdateUserAddress, nameof(request.UpdateUserAddress));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var identityResult = identity.GetCurrentIdentity();

        if (!identityResult.IsSuccess) return Result.Forbidden("Invalid identity");

        var validationResult = await validator.ValidateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            logger.LogValidationFailed("Update user address", string.Join(", ", errors));
            return Result.Invalid(validationResult.Errors.ToValidationErrors());
        }

        try
        {
            var user = await repository.GetUserByKeycloakIdAsync(identityResult.Value.KeycloakUserId, cancellationToken)
                .ConfigureAwait(false);

            Guard.Against.Null(user, nameof(user));

            if (user.Address == null)
            {
                var address = new CreateAddressDto
                {
                    Street = request.UpdateUserAddress.Street,
                    Apartment = request.UpdateUserAddress.Apartment,
                    City = request.UpdateUserAddress.City,
                    Country = request.UpdateUserAddress.Country,
                    Zip = request.UpdateUserAddress.Zip,
                    Region = request.UpdateUserAddress.Region
                };

                user.SetInitialAddress(address);
            }
            else
            {
                var address = new UpdateAddressDto
                {
                    Street = request.UpdateUserAddress.Street,
                    Apartment = request.UpdateUserAddress.Apartment,
                    City = request.UpdateUserAddress.City,
                    Country = request.UpdateUserAddress.Country,
                    Zip = request.UpdateUserAddress.Zip,
                    Region = request.UpdateUserAddress.Region
                };

                user.UpdateAddress(address);
            }

            await repository.UpdateUserAsync(user, cancellationToken).ConfigureAwait(false);
            await eventDispatcher.DispatchEventsAsync(user, cancellationToken).ConfigureAwait(false);

            return Result.Success(mapper.Map<AddressResponse?>(user.Address));
        }
        catch (NotFoundException)
        {
            logger.LogOperationFailed("Update user address", "keycloakId", identityResult.Value.KeycloakUserId, null);
            return Result.Error("Failed to update user address");
        }
    }
}
