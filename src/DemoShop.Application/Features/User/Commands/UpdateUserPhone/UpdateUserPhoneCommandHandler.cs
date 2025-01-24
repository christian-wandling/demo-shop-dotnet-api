using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.Common.Extensions;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Commands.UpdateUserPhone;

public sealed class UpdateUserPhoneCommandHandler(
    IUserRepository repository,
    ILogger<UpdateUserPhoneCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<UpdateUserPhoneCommand> validator
)
    : IRequestHandler<UpdateUserPhoneCommand, Result<string?>>
{
    public async Task<Result<string?>> Handle(UpdateUserPhoneCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UserIdentity, nameof(request.UserIdentity));
        Guard.Against.NullOrWhiteSpace(request.NewPhoneNumber, nameof(request.NewPhoneNumber));

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
            var user = await repository.GetUserByKeycloakIdAsync(request.UserIdentity.KeycloakId, cancellationToken)
                .ConfigureAwait(false);

            Guard.Against.Null(user, nameof(user));
            user.UpdatePhone(request.NewPhoneNumber);

            await repository.UpdateUserPhoneAsync(user, cancellationToken).ConfigureAwait(false);
            await eventDispatcher.DispatchEventsAsync(user, cancellationToken).ConfigureAwait(false);

            return Result<string?>.Success(user.Phone?.Value);
        }
        catch (NotFoundException)
        {
            logger.LogOperationFailed("Update user phone", "keycloakId", request.UserIdentity.KeycloakId, null);
            return Result.Error("Failed to update user phone");
        }
    }
}
