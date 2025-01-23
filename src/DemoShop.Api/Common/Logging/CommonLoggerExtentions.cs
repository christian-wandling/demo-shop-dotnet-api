using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Logging;

namespace DemoShop.Api.Common.Logging;

public static class CommonLoggerExtensions
{
    private static readonly Action<ILogger, string, string, Exception?> ResultError =
        LoggerMessage.Define<string, string>(
            LogLevel.Warning,
            LoggerEventIds.ApiResultError,
            "{Operation} failed: {Errors}");

    private static readonly Action<ILogger, string, Exception?> ResultSuccess =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.ApiResultError,
            "{Operation} completed successfully");

    public static void LogResult<T>(
        this ILogger logger,
        Result<T> result,
        string operation)
    {
        Guard.Against.Null(result, nameof(result));
        Guard.Against.NullOrWhiteSpace(operation, nameof(operation));

        if (result.IsSuccess)
        {
            ResultSuccess(logger, operation, null);
            return;
        }

        ResultError(logger, operation, string.Join(", ", result.Errors), null);
    }
}
