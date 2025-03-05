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
using Serilog;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;

public sealed class UpdateCartItemQuantityCommandHandler(
    IMapper mapper,
    ICurrentShoppingSessionAccessor currentSession,
    IShoppingSessionRepository repository,
    ILogger logger,
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

        var sessionResult = await currentSession.GetCurrent(cancellationToken);

        if (!sessionResult.IsSuccess)
            return sessionResult.Map();

        try
        {
            LogCommandStarted(logger, request.Id, sessionResult.Value.Id);

            var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);
            if (!validationResult.IsSuccess)
            {
                LogCommandError(logger, request.Id, sessionResult.Value.Id);
                return validationResult.Map();
            }

            var unsavedResult = sessionResult.Value.UpdateCartItem(request.Id, request.UpdateCartItem.Quantity);
            if (!unsavedResult.IsSuccess)
            {
                LogCommandError(logger, request.Id, sessionResult.Value.Id);
                return unsavedResult.Map();
            }

            var savedResult = await SaveChanges(sessionResult.Value, cancellationToken);
            if (!savedResult.IsSuccess)
            {
                LogCommandError(logger, request.Id, sessionResult.Value.Id);
                return savedResult.Map();
            }

            var savedCartItem = savedResult.Value.CartItems.FirstOrDefault(c => c.Id == request.Id);
            if (savedCartItem is null)
            {
                LogCommandError(logger, request.Id, sessionResult.Value.Id);
                Result.NotFound("Cart item not found");
            }

            LogCommandSuccess(logger, request.Id, sessionResult.Value.Id);
            return Result.Success(mapper.Map<UpdateCartItemQuantityResponse>(savedCartItem));
        }
        catch (InvalidOperationException ex)
        {
            LogInvalidOperationException(logger, ex.Message, ex);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            LogDatabaseException(logger, ex.Message, ex);
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

    private static void LogCommandStarted(ILogger logger, int cartItemId, int sessionId) => logger
        .ForContext("EventId", LoggerEventId.UpdateCartItemQuantityCommandStarted)
        .Information(
            "Starting to update quantity of cart item with Id {CartItemId} for shopping session {SessionId}",
            cartItemId, sessionId);

    private static void LogCommandSuccess(ILogger logger, int cartItemId, int sessionId) => logger
        .ForContext("EventId", LoggerEventId.UpdateCartItemQuantityCommandSuccess)
        .Information(
            "Successfully updated quantity of cart item with Id {CartItemId} for shopping session {SessionId}",
            cartItemId, sessionId);

    private static void LogCommandError(ILogger logger, int cartItemId, int sessionId) => logger
        .ForContext("EventId", LoggerEventId.UpdateCartItemQuantityCommandError)
        .Error("Error updating quantity of cart item with Id {CartItemId} for shopping session {SessionId}",
            cartItemId, sessionId);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.UpdateShoppingSessionDatabaseException)
        .Error(ex, "Database error occurred while updating cart item quantity. Error: {ErrorMessage}", errorMessage);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.UpdateShoppingSessionDomainException)
        .Error(ex, "Invalid operation while updating cart item quantity. Error: {ErrorMessage}", errorMessage);
}
