using DemoShop.Domain.Common.Logging;

namespace DemoShop.Api.Features.Users;

public static class UserLoggerExtensions
{
    private static readonly Action<ILogger, Exception?> GetCurrentUserStarted =
        LoggerMessage.Define(
            LogLevel.Information,
            LoggerEventIds.ApiGetCurrentUserStarted,
            "Getting current user information");

    private static readonly Action<ILogger, string, Exception?> UserIdentityFailed =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            LoggerEventIds.ApiUserIdentityFailed,
            "Failed to get user identity: {Reason}");

    public static void LogGetCurrentUserStarted(this ILogger logger) =>
        GetCurrentUserStarted(logger, null);

    public static void LogUserIdentityFailed(this ILogger logger, string reason) =>
        UserIdentityFailed(logger, reason, null);
}
