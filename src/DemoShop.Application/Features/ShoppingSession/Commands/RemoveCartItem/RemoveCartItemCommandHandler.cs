#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
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

namespace DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;

public sealed class RemoveCartItemCommandHandler(
    ICurrentShoppingSessionAccessor currentSession,
    IShoppingSessionRepository repository,
    IDomainEventDispatcher eventDispatcher,
    ILogger logger,
    IValidator<RemoveCartItemCommand> validator,
    IValidationService validationService
)
    : IRequestHandler<RemoveCartItemCommand, Result>
{
    public async Task<Result> Handle(RemoveCartItemCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NegativeOrZero(request.Id, nameof(request.Id));

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

            var unsavedResult = sessionResult.Value.RemoveCartItem(request.Id);
            if (!unsavedResult.IsSuccess)
            {
                LogCommandError(logger, request.Id, sessionResult.Value.Id);
                return unsavedResult;
            }

            var savedResult = await SaveChanges(sessionResult, cancellationToken);
            if (!savedResult.IsSuccess)
            {
                LogCommandError(logger, request.Id, savedResult.Value.Id);
                return savedResult.Map();
            }

            LogCommandSuccess(logger, request.Id, sessionResult.Value.Id);
            return Result.NoContent();
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

    private static void LogCommandStarted(ILogger logger, int carItemId, int sessionId) => logger
        .ForContext("EventId", LoggerEventId.RemoveCartItemCommandStarted)
        .Debug("Starting to remove cart item with Id {CartItemId} from shopping session {SessionId}",
            carItemId, sessionId);

    private static void LogCommandSuccess(ILogger logger, int carItemId, int sessionId) => logger
        .ForContext("EventId", LoggerEventId.RemoveCartItemCommandSuccess)
        .Information(
            "Successfully removed cart item with Id {CartItemId} from shopping session with Id {SessionId}",
            carItemId, sessionId);

    private static void LogCommandError(ILogger logger, int carItemId, int sessionId) => logger
        .ForContext("EventId", LoggerEventId.RemoveCartItemCommandError)
        .Error("Error removing cart item with id {CartItemId} from shopping session {SessionId}",
            carItemId, sessionId);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.UpdateShoppingSessionDatabaseException)
        .Error(ex, "Database error occurred while updating shopping session. Error: {ErrorMessage}", errorMessage);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.UpdateShoppingSessionDomainException)
        .Error(ex, "Invalid operation while updating shopping session. Error: {ErrorMessage}", errorMessage);
}
