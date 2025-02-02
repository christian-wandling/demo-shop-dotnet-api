using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Common.Extensions;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;

public sealed class UpdateCartItemQuantityCommandHandler(
    IMapper mapper,
    ICurrentShoppingSessionAccessor currentSession,
    IShoppingSessionRepository repository,
    ILogger<UpdateCartItemQuantityCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<UpdateCartItemQuantityCommand> validator
)
    : IRequestHandler<UpdateCartItemQuantityCommand, Result<UpdateCartItemQuantityResponse>>
{
    public async Task<Result<UpdateCartItemQuantityResponse>> Handle(UpdateCartItemQuantityCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UpdateCartItem, nameof(request));

        var validationResult = await validator.ValidateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            logger.LogValidationFailed("Add Cart item to Shopping session", string.Join(", ", errors));

            return Result.Invalid(validationResult.Errors.ToValidationErrors());
        }

        var currentSessionResult = await currentSession.GetCurrent(cancellationToken).ConfigureAwait(false);

        Guard.Against.Null(currentSessionResult, nameof(currentSessionResult));

        var updatedCartItem = currentSessionResult.Value.UpdateCartItem(
            request.Id,
            request.UpdateCartItem.Quantity
        );

        await repository.UpdateSessionAsync(currentSessionResult.Value, cancellationToken)
            .ConfigureAwait(false);

        await eventDispatcher.DispatchEventsAsync(currentSessionResult.Value, cancellationToken).ConfigureAwait(false);

        return Result.Success(mapper.Map<UpdateCartItemQuantityResponse>(updatedCartItem));
    }
}
