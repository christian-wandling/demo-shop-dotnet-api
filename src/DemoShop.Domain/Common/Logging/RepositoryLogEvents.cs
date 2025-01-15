using Microsoft.Extensions.Logging;

namespace DemoShop.Domain.Common.Logging;

public static class RepositoryLogEvents
{
    internal static readonly Action<ILogger, string, string, string, Exception?> DatabaseError =
        DefineLogMessage(
            LogLevel.Error,
            100,
            "Database error while {Operation} {EntityType} [{Identifier}]");

    internal static readonly Action<ILogger, string, string, string, Exception?> OperationError =
        DefineLogMessage(
            LogLevel.Error,
            101,
            "Operation error while {Operation} {EntityType} [{Identifier}]");

    internal static readonly Action<ILogger, string, string, string, Exception?> NotFoundError =
        DefineLogMessage(
            LogLevel.Warning,
            102,
            "{EntityType} not found while {Operation} [{Identifier}]");

    internal static readonly Action<ILogger, string, string, string, Exception?> DeleteFailedError =
        DefineLogMessage(
            LogLevel.Warning,
            103,
            "{EntityType} delete failed while {Operation} [{Identifier}]");

    private static Action<ILogger, string, string, string, Exception?> DefineLogMessage(
        LogLevel level,
        int eventId,
        string messageTemplate
    ) => LoggerMessage.Define<string, string, string>(
        level,
        new EventId(eventId, "RepositoryError"),
        messageTemplate
    );
}
