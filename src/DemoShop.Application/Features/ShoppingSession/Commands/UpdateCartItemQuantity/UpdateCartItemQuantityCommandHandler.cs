#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;

public sealed class UpdateCartItemQuantityCommandHandler(
    IMapper mapper,
    ICurrentShoppingSessionAccessor currentSession,
    IShoppingSessionRepository repository,
    ILogger<UpdateCartItemQuantityCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<UpdateCartItemQuantityCommand> validator,
    IValidationService validationService
)
    : IRequestHandler<UpdateCartItemQuantityCommand, Result<UpdateCartItemQuantityResponse>>
{
    public async Task<Result<UpdateCartItemQuantityResponse>> Handle(UpdateCartItemQuantityCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NegativeOrZero(request.Id, nameof(request.Id));
        Guard.Against.Null(request.UpdateCartItem, nameof(request.UpdateCartItem));

        var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);

        if (!validationResult.IsSuccess)
            return validationResult.Map();

        var sessionResult = await currentSession.GetCurrent(cancellationToken);

        if (!sessionResult.IsSuccess)
            return sessionResult.Map();

        try
        {
            var unsavedResult = sessionResult.Value.UpdateCartItem(request.Id, request.UpdateCartItem.Quantity);

            if (!unsavedResult.IsSuccess)
                return unsavedResult.Map();

            var savedResult = await SaveChanges(sessionResult.Value, cancellationToken);

            return savedResult.IsSuccess
                ? Result.Success(mapper.Map<UpdateCartItemQuantityResponse>(savedResult.Value))
                : savedResult.Map();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogOperationFailed("Update cart item quantity", "Id", $"{request.Id}", ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<Result<ShoppingSessionEntity>> SaveChanges(ShoppingSessionEntity unsavedSession,
        CancellationToken cancellationToken)
    {
        var savedSession = await repository.UpdateSessionAsync(unsavedSession, cancellationToken);

        await eventDispatcher.DispatchEventsAsync(unsavedSession, cancellationToken);
        return Result.Success(savedSession);
    }
}
