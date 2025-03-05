#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Features.ShoppingSessions;

public class ShoppingSessionRepository(ApplicationDbContext context, ILogger logger) : IShoppingSessionRepository
{
    public async Task<ShoppingSessionEntity?> GetSessionByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(userId, nameof(userId));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));
        LogGetShoppingSessionByUserIdStarted(logger, userId);

        var result = await context.Query<ShoppingSessionEntity>()
            .Include(s => s.CartItems)
            .ThenInclude(c => c.Product)
            .ThenInclude(p => p!.Images)
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

        if (result is null)
            LogGetShoppingSessionByUserIdNotFound(logger, userId);
        else
            LogGetShoppingSessionByUserIdSuccess(logger, result.Id);

        return result;
    }

    public async Task<ShoppingSessionEntity?> CreateSessionAsync(ShoppingSessionEntity session,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(session, nameof(session));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));
        LogCreateShoppingSessionStarted(logger, session.UserId);

        var createdSession = await context.Set<ShoppingSessionEntity>()
            .AddAsync(session, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        LogCreateShoppingSessionSuccess(logger, createdSession.Entity.Id, createdSession.Entity.UserId);
        return createdSession.Entity;
    }

    public async Task<ShoppingSessionEntity> UpdateSessionAsync(
        ShoppingSessionEntity session,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(session, nameof(session));
        LogUpdateShoppingSessionStarted(logger, session.Id);

        var entry = context.Set<ShoppingSessionEntity>().Update(session);
        await context.SaveChangesAsync(cancellationToken);
        await entry.ReloadAsync(cancellationToken);

        LogUpdateShoppingSessionSuccess(logger, entry.Entity.Id);
        return entry.Entity;
    }

    public async Task<bool> DeleteSessionAsync(
        ShoppingSessionEntity session,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(session, nameof(session));
        LogDeleteShoppingSessionStarted(logger, session.Id);

        context.Set<ShoppingSessionEntity>().Remove(session);
        var affected = await context.SaveChangesAsync(cancellationToken);
        var isDeleted = affected > 0;

        if (isDeleted)
            LogDeleteShoppingSessionSuccess(logger, session.Id);
        else
            LogDeleteShoppingSessionFailed(logger, session.Id);

        return isDeleted;
    }

    private static void LogGetShoppingSessionByUserIdStarted(ILogger logger, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.GetShoppingSessionByUserIdStarted)
            .Debug("Attempting to get shoppingSession with UserId {UserId}", userId);

    private static void LogGetShoppingSessionByUserIdSuccess(ILogger logger, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.GetShoppingSessionByUserIdSuccess)
            .Debug("Attempting to get shoppingSession with UserId {UserId} completed successfully", userId);

    private static void LogGetShoppingSessionByUserIdNotFound(ILogger logger, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.GetShoppingSessionByUserIdNotFound)
            .Warning("ShoppingSession with UserId {UserId} not found in database", userId);

    private static void LogCreateShoppingSessionStarted(ILogger logger, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.CreateShoppingSessionStarted)
            .Debug("Attempting to create shoppingSession for user {UserId}", userId);

    private static void LogCreateShoppingSessionSuccess(ILogger logger, int sessionId, int userId) =>
        logger
            .ForContext("EventId", LoggerEventId.CreateShoppingSessionSuccess)
            .Debug("Successfully created shoppingSession with Id {SessionId} for user {UserId}",
                sessionId, userId);

    private static void LogUpdateShoppingSessionStarted(ILogger logger, int sessionId) =>
        logger
            .ForContext("EventId", LoggerEventId.UpdateShoppingSessionStarted)
            .Debug("Attempting to update shoppingSession with Id {SessionId}", sessionId);

    private static void LogUpdateShoppingSessionSuccess(ILogger logger, int sessionId) =>
        logger
            .ForContext("EventId", LoggerEventId.UpdateShoppingSessionSuccess)
            .Debug("Successfully updated shoppingSession with Id {SessionId}", sessionId);

    private static void LogDeleteShoppingSessionStarted(ILogger logger, int sessionId) =>
        logger
            .ForContext("EventId", LoggerEventId.DeleteShoppingSessionStarted)
            .Debug("Attempting to delete shoppingSession with Id {SessionId}", sessionId);

    private static void LogDeleteShoppingSessionSuccess(ILogger logger, int sessionId) =>
        logger
            .ForContext("EventId", LoggerEventId.DeleteShoppingSessionSuccess)
            .Debug("Successfully deleted shoppingSession with Id {SessionId}", sessionId);

    private static void LogDeleteShoppingSessionFailed(ILogger logger, int sessionId) =>
        logger
            .ForContext("EventId", LoggerEventId.DeleteShoppingSessionFailed)
            .Error("Failed to delete shoppingSession with Id {SessionId}", sessionId);
}
