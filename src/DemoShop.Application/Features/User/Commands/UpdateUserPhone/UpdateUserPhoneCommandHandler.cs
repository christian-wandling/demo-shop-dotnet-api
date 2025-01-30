using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Common.Extensions;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Commands.UpdateUserPhone;

public sealed class UpdateUserPhoneCommandHandler(
    IMapper mapper,
    IUserIdentityAccessor identity,
    IUserRepository repository,
    ILogger<UpdateUserPhoneCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<UpdateUserPhoneCommand> validator
)
    : IRequestHandler<UpdateUserPhoneCommand, Result<UserPhoneResponse>>
{
    public async Task<Result<UserPhoneResponse>> Handle(UpdateUserPhoneCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UpdateUserPhone, nameof(request.UpdateUserPhone));
        Guard.Against.Null(cancellationToken, nameof(CancellationToken));

        var identityResult = identity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
        {
            return Result.Forbidden("Invalid identity");
        }

        var validationResult = await validator.ValidateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            logger.LogValidationFailed("Update user phone", string.Join(", ", errors));
            return Result.Invalid(validationResult.Errors.ToValidationErrors());
        }

        try
        {
            var user = await repository.GetUserByKeycloakIdAsync(identityResult.Value.KeycloakUserId, cancellationToken)
                .ConfigureAwait(false);

            Guard.Against.Null(user, nameof(user));
            user.UpdatePhone(request.UpdateUserPhone.Phone);

            await repository.UpdateUserAsync(user, cancellationToken).ConfigureAwait(false);
            await eventDispatcher.DispatchEventsAsync(user, cancellationToken).ConfigureAwait(false);

            return Result.Success(mapper.Map<UserPhoneResponse>(user));
        }
        catch (NotFoundException)
        {
            logger.LogOperationFailed("Update user phone", "keycloakId", identityResult.Value.KeycloakUserId, null);
            return Result.Error("Failed to update user phone");
        }
    }
}
