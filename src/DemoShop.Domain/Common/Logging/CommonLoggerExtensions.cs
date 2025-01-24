using Microsoft.Extensions.Logging;

namespace DemoShop.Domain.Common.Logging;

public static class CommonLoggerExtensions
{
    private static readonly Action<ILogger, string, string, string, Exception?> OperationFailed =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            LoggerEventIds.OperationFailed,
            "{Operation} with {IdentifierType}: {Identifier} failed");

    public static void LogOperationFailed(
        this ILogger logger,
        string operation,
        string identifierType,
        string identifier,
        Exception? ex
    ) =>
        OperationFailed(logger, operation, identifierType, identifier, ex);

    private static readonly Action<ILogger, string, string, string, Exception?> OperationSuccess =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            LoggerEventIds.OperationSuccess,
            "{Operation} with {IdentifierType}: {Identifier} succeeded");

    public static void LogOperationSuccess(
        this ILogger logger,
        string operation,
        string identifierType,
        string identifier
    ) =>
        OperationSuccess(logger, operation, identifierType, identifier, null);

    private static readonly Action<ILogger, string, string, Exception?> ValidationFailed =
        LoggerMessage.Define<string, string>(
            LogLevel.Warning,
            LoggerEventIds.ValidationFailed,
            "Validation failed for {Operation}, Reason: {Errors}");

    public static void LogValidationFailed(this ILogger logger, string operation, string errors) =>
        ValidationFailed(logger, operation, errors, null);

    private static readonly Action<ILogger, string, Exception?> AuthFailed =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            LoggerEventIds.AuthFailed,
            "Auth failed, Reason: {Error}");

    public static void LogAuthFailed(this ILogger logger, string error) =>
        AuthFailed(logger, error, null);
}
