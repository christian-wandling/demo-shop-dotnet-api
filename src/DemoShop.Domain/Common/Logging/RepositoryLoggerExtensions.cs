using Microsoft.Extensions.Logging;

namespace DemoShop.Domain.Common.Logging;

public static class RepositoryLoggerExtensions
{
    public static void LogDatabaseError<T>(
        this ILogger logger,
        string operation,
        Exception ex,
        string? identifier = null)
        => RepositoryLogEvents.DatabaseError(
            logger,
            operation,
            typeof(T).Name,
            identifier ?? "collection",
            ex);

    public static void LogOperationError<T>(
        this ILogger logger,
        string operation,
        Exception ex,
        string? identifier = null)
        => RepositoryLogEvents.OperationError(
            logger,
            operation,
            typeof(T).Name,
            identifier ?? "collection",
            ex);

    public static void LogNotFoundError<T>(
        this ILogger logger,
        string operation,
        string? identifier = null)
        => RepositoryLogEvents.NotFoundError(
            logger,
            operation,
            typeof(T).Name,
            identifier ?? "collection",
            null);

    public static void LogDeleteFailedError<T>(
        this ILogger logger,
        string operation,
        string? identifier = null)
        => RepositoryLogEvents.DeleteFailedError(
            logger,
            operation,
            typeof(T).Name,
            identifier ?? "collection",
            null);
}
