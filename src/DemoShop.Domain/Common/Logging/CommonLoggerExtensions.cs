#region

using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Domain.Common.Logging;

public static class CommonLoggerExtensions
{
    private static readonly Action<ILogger, string, string, string, Exception?> OperationFailed =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            LoggerEventIds.OperationFailed,
            "{Operation} with {IdentifierType}: {Identifier} failed");

    private static readonly Action<ILogger, string, string, string, Exception?> OperationSuccess =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            LoggerEventIds.OperationSuccess,
            "{Operation} with {IdentifierType}: {Identifier} succeeded");

    private static readonly Action<ILogger, string, string, Exception?> ValidationFailed =
        LoggerMessage.Define<string, string>(
            LogLevel.Warning,
            LoggerEventIds.ValidationFailed,
            "Validation failed for {Operation}, Reason: {Errors}");

    private static readonly Action<ILogger, string, Exception?> AuthFailed =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            LoggerEventIds.AuthFailed,
            "Auth failed, Reason: {Error}");

    private static readonly Action<ILogger, string, Exception?> DomainEvent =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.DomainEvent,
            "{Message}");

    private static readonly Action<ILogger, string, Exception?> DomainException =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.DomainException,
            "{Message}");

    public static void LogOperationFailed(
        this ILogger logger,
        string operation,
        string identifierType,
        string identifier,
        Exception? ex
    ) =>
        OperationFailed(logger, operation, identifierType, identifier, ex);

    public static void LogOperationSuccess(
        this ILogger logger,
        string operation,
        string identifierType,
        string identifier
    ) =>
        OperationSuccess(logger, operation, identifierType, identifier, null);

    public static void LogValidationFailed(this ILogger logger, string operation, string errors) =>
        ValidationFailed(logger, operation, errors, null);

    public static void LogAuthFailed(this ILogger logger, string error) =>
        AuthFailed(logger, error, null);

    public static void LogDomainEvent(
        this ILogger logger,
        string message
    ) =>
        DomainEvent(logger, message, null);

    public static void LogDomainException(
        this ILogger logger,
        string message
    ) =>
        DomainException(logger, message, null);
}
