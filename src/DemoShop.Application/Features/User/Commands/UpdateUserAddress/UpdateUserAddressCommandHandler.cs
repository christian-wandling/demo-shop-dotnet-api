using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.Common.Extensions;
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
    IUserRepository repository,
    ILogger<UpdateUserAddressCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<UpdateUserAddressCommand> validator
)
    : IRequestHandler<UpdateUserAddressCommand, Result<AddressEntity?>>
{
    public async Task<Result<AddressEntity?>> Handle(UpdateUserAddressCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UserIdentity, nameof(request.UserIdentity));
        Guard.Against.Null(request.Address, nameof(request.Address));

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
            var user = await repository.GetUserByKeycloakIdAsync(request.UserIdentity.KeycloakUserId, cancellationToken)
                .ConfigureAwait(false);

            Guard.Against.Null(user, nameof(user));

            if (user.Address == null)
            {
                var address = new CreateAddressDto
                {
                    Street = request.Address.Street,
                    Apartment = request.Address.Apartment,
                    City = request.Address.City,
                    Country = request.Address.Country,
                    Zip = request.Address.Zip,
                    Region = request.Address.Region,
                };

                user.SetInitialAddress(address);
            }
            else
            {
                var address = new UpdateAddressDto
                {
                    Street = request.Address.Street,
                    Apartment = request.Address.Apartment,
                    City = request.Address.City,
                    Country = request.Address.Country,
                    Zip = request.Address.Zip,
                    Region = request.Address.Region,
                };

                user.UpdateAddress(address);
            }

            await repository.UpdateUserAddressAsync(user, cancellationToken).ConfigureAwait(false);
            await eventDispatcher.DispatchEventsAsync(user, cancellationToken).ConfigureAwait(false);

            return Result<AddressEntity?>.Success(user.Address);
        }
        catch (NotFoundException)
        {
            logger.LogOperationFailed("Update user address", "keycloakId", request.UserIdentity.KeycloakUserId, null);
            return Result.Error("Failed to update user address");
        }
    }
}
