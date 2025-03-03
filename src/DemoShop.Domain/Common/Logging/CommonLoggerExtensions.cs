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

    public static void LogAuthSuccess(this ILogger logger, string keycloakUserId)
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Information(
            "[{EventId}] Authentication success for user with KeycloakUserId {KeycloakUserId}",
            LoggerEventIds.AuthSuccess,
            keycloakUserId);
    }

    public static void LogAuthenticationFailed(this ILogger logger, string error)
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Error(
            "[{EventId}] Authentication failed, Reason: {Error}",
            LoggerEventIds.AuthenticationFailed,
            error);
    }

    public static void LogAuthorizationDenied(
        this ILogger logger,
        string error
    )
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Error(
            "[{EventId}] Authorization denied, Reason: {Error}",
            LoggerEventIds.AuthorizationDenied,
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
}
