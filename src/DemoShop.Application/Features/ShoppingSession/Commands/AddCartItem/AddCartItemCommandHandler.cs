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

namespace DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;

public sealed class AddCartItemCommandHandler(
    IMapper mapper,
    ICurrentShoppingSessionAccessor currentSession,
    IShoppingSessionRepository repository,
    ILogger<AddCartItemCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<AddCartItemCommand> validator
)
    : IRequestHandler<AddCartItemCommand, Result<CartItemResponse>>
{
    public async Task<Result<CartItemResponse>> Handle(AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.AddCartItem, nameof(request));

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

        currentSessionResult.Value.AddCartItem(request.AddCartItem.ProductId);

        await repository.UpdateSessionAsync(currentSessionResult.Value, cancellationToken)
            .ConfigureAwait(false);

        currentSessionResult = await currentSession.GetCurrent(cancellationToken)
            .ConfigureAwait(false);

        var addedCartItem = currentSessionResult.Value.CartItems.FirstOrDefault(c =>
            c.ProductId == request.AddCartItem.ProductId
        );

        await eventDispatcher.DispatchEventsAsync(currentSessionResult.Value, cancellationToken).ConfigureAwait(false);

        return Result.Success(
            mapper.Map<CartItemResponse>(addedCartItem)
        );
    }
}
