#region

using Serilog;

#endregion

namespace DemoShop.Domain.Common.Logging;

public static class CommonLoggerExtensions
{
    public static void LogOperationFailed(
        this ILogger logger,
        string operation,
        string identifierType,
        string identifier,
        Exception? ex = null
    )
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Information(ex,
            "[{EventId}] {Operation} with {IdentifierType}: {Identifier} failed",
            LoggerEventIds.OperationFailed,
            operation,
            identifierType,
            identifier);
    }

    public static void LogOperationSuccess(
        this ILogger logger,
        string operation,
        string identifierType,
        string identifier
    )
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Information(
            "[{EventId}] {Operation} with {IdentifierType}: {Identifier} succeeded",
            LoggerEventIds.OperationSuccess,
            operation,
            identifierType,
            identifier);
    }

    public static void LogValidationFailed(
        this ILogger logger,
        string operation,
        string errors)
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Warning(
            "[{EventId}] Validation failed for {Operation}, Reason: {Errors}",
            LoggerEventIds.ValidationFailed,
            operation,
            errors);
    }

    public static void LogAuthFailed(
        this ILogger logger,
        string error)
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Error(
            "[{EventId}] Auth failed, Reason: {Error}",
            LoggerEventIds.AuthFailed,
            error);
    }

    public static void LogDomainEvent(
        this ILogger logger,
        string message
    )
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Information(
            "[{EventId}] {Message}",
            LoggerEventIds.DomainEvent,
            message);
    }

    public static void LogDomainException(
        this ILogger logger,
        string message
    )
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Information(
            "[{EventId}] {Message}",
            LoggerEventIds.DomainException,
            message);
    }

    public static void LogCacheWrite(
        this ILogger logger,
        string key
    )
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Information(
            "[{EventId}] Cache write for {Key}",
            LoggerEventIds.CacheWrite,
            key);
    }

    public static void LogCacheHit(
        this ILogger logger,
        string key
    )
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Information(
            "[{EventId}] Cache hit for {Key}",
            LoggerEventIds.CacheHit,
            key);
    }

    public static void LogCacheMiss(
        this ILogger logger,
        string key
    )
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Information(
            "[{EventId}] Cache miss for {Key}",
            LoggerEventIds.CacheMiss,
            key);
    }
}
