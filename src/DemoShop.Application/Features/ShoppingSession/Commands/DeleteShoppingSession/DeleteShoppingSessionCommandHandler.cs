#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.DeleteShoppingSession;

public sealed class DeleteShoppingSessionCommandHandler(
    IShoppingSessionRepository repository,
    IDomainEventDispatcher eventDispatcher,
    ILogger logger
)
    : IRequestHandler<DeleteShoppingSessionCommand, Result>
{
    public async Task<Result> Handle(DeleteShoppingSessionCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Session, nameof(request.Session));

        try
        {
            LogCommandStarted(logger, request.Session.Id);

            var deleted = await repository.DeleteSessionAsync(request.Session, cancellationToken);
            if (!deleted)
            {
                LogCommandError(logger, request.Session.Id);
                return Result.Error("The shopping session was not deleted");
            }

            await eventDispatcher.DispatchEventsAsync(request.Session, cancellationToken);

            LogCommandSuccess(logger, request.Session.Id);
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

    private static void LogCommandStarted(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.DeleteShoppingSessionCommandStarted)
        .Debug("Starting to delete shopping session with Id {Id}", id);

    private static void LogCommandSuccess(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.DeleteShoppingSessionCommandSuccess)
        .Information("Successfully deleted shopping session with Id {Id}", id);

    private static void LogCommandError(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.DeleteShoppingSessionCommandError)
        .Error("Error deleting shopping session with Id {Id}", id);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.DeleteShoppingSessionDatabaseException)
        .Error(ex, "Database error occurred while deleting shopping session. Error: {ErrorMessage}", errorMessage);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.DeleteShoppingSessionDomainException)
        .Error(ex, "Invalid operation while deleting shopping session. Error: {ErrorMessage}", errorMessage);
}
