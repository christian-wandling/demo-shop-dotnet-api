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

namespace DemoShop.Application.Features.User.Commands.UpdateUserPhone;

public sealed class UpdateUserPhoneCommandHandler(
    IMapper mapper,
    IUserIdentityAccessor identity,
    IUserRepository repository,
    ILogger<UpdateUserPhoneCommandHandler> logger,
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

        var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);

        if (!validationResult.IsSuccess)
            return validationResult.Map();

        var identityResult = identity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
            return identityResult.Map<IUserIdentity, UserPhoneResponse>(null);

        try
        {
            var user = await repository.GetUserByKeycloakIdAsync(
                identityResult.Value.KeycloakUserId,
                cancellationToken
            );

            if (user is null)
                return Result.NotFound("User not found");

            var unsavedResult = user.UpdatePhone(request.UpdateUser.Phone);

            if (!unsavedResult.IsSuccess)
                return unsavedResult.Map();

            var savedResult = await SaveChanges(user, cancellationToken);

            return savedResult.IsSuccess
                ? Result.Success(mapper.Map<UserPhoneResponse>(user))
                : savedResult.Map();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogOperationFailed("Update user phone", "KeycloakUserId", identityResult.Value.KeycloakUserId, ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<Result<UserEntity>> SaveChanges(UserEntity unsavedUser, CancellationToken cancellationToken)
    {
        var savedUser = await repository.UpdateUserAsync(unsavedUser, cancellationToken);

        await eventDispatcher.DispatchEventsAsync(unsavedUser, cancellationToken);
        return Result.Success(savedUser);
    }
}
