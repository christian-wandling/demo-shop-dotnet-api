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

namespace DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;

public sealed class AddCartItemCommandHandler(
    IMapper mapper,
    ICurrentShoppingSessionAccessor currentSession,
    IShoppingSessionRepository repository,
    ILogger<AddCartItemCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<AddCartItemCommand> validator,
    IValidationService validationService
)
    : IRequestHandler<AddCartItemCommand, Result<CartItemResponse>>
{
    public async Task<Result<CartItemResponse>> Handle(AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.AddCartItem, nameof(request));

        var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);

        if (!validationResult.IsSuccess)
            return validationResult.Map();

        var sessionResult = await currentSession.GetCurrent(cancellationToken);

        if (!sessionResult.IsSuccess)
            return sessionResult.Map();

        try
        {
            var unsavedResult = sessionResult.Value.AddCartItem(request.AddCartItem.ProductId);

            if (!unsavedResult.IsSuccess)
                return unsavedResult.Map();

            var savedResult = await SaveChanges(sessionResult, cancellationToken);

            if (!savedResult.IsSuccess)
                return savedResult.Map();

            var savedCartItem = savedResult.Value.CartItems.FirstOrDefault(c =>
                c.ProductId == request.AddCartItem.ProductId
            );

            return savedCartItem is null
                ? Result.Error("Could not add cart item")
                : Result.Success(mapper.Map<CartItemResponse>(savedCartItem)
                );
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogOperationFailed("Add cart item", "ProductId", $"{request.AddCartItem.ProductId}", ex);
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
