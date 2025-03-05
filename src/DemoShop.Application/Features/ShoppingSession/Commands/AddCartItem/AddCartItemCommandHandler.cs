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

namespace DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;

public sealed class AddCartItemCommandHandler(
    IMapper mapper,
    ICurrentShoppingSessionAccessor currentSession,
    IShoppingSessionRepository repository,
    ILogger logger,
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

        var sessionResult = await currentSession.GetCurrent(cancellationToken);

        if (!sessionResult.IsSuccess)
            return sessionResult.Map();

        try
        {
            LogCommandStarted(logger, sessionResult.Value.Id);
            var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);

            if (!validationResult.IsSuccess)
            {
                LogCommandError(logger, sessionResult.Value.Id);
                return validationResult.Map();
            }

            var unsavedResult = sessionResult.Value.AddCartItem(request.AddCartItem.ProductId);
            if (!unsavedResult.IsSuccess)
            {
                LogCommandError(logger, sessionResult.Value.Id);
                return unsavedResult.Map();
            }

            var savedResult = await SaveChanges(sessionResult, cancellationToken);
            if (!savedResult.IsSuccess)
            {
                LogCommandError(logger, sessionResult.Value.Id);
                return savedResult.Map();
            }

            var savedCartItem = savedResult.Value.CartItems.FirstOrDefault(c =>
                c.ProductId == request.AddCartItem.ProductId
            );
            if (savedCartItem is null)
            {
                LogCommandError(logger, sessionResult.Value.Id);
                return Result.Error("Could not add cart item");
            }

            LogCommandSuccess(logger, savedCartItem.Id, savedCartItem.ShoppingSessionId);
            return Result.Success(mapper.Map<CartItemResponse>(savedCartItem));
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

    private static void LogCommandStarted(ILogger logger, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.AddCartItemCommandStarted)
            .Information("Starting to add cart item to shopping session {SessionId}", sessionId);

    private static void LogCommandSuccess(ILogger logger, int carItemId, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.AddCartItemCommandSuccess)
            .Information(
                "Successfully added cart item with Id {CartItemId} to shopping session with Id {SessionId}",
                carItemId, sessionId);

    private static void LogCommandError(ILogger logger, int sessionId) =>
        logger.ForContext("EventId", LoggerEventIds.AddCartItemCommandError)
            .Information("Error adding cart item with to shopping session {SessionId}", sessionId);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) =>
        logger.Error(ex, "Database error occurred while updating shopping session. Error: {ErrorMessage} {@EventId}",
            errorMessage, LoggerEventIds.UpdateShoppingSessionDatabaseException);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) =>
        logger.Error(ex, "Invalid operation while updating shopping session. Error: {ErrorMessage} {@EventId}",
            errorMessage, LoggerEventIds.UpdateShoppingSessionDomainException);
}
