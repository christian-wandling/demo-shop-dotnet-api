using DemoShop.Domain.Common.Logging;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Common.Logging;

public static class CommonLoggerExtensions
{
    private static readonly Action<ILogger, Exception?> IdentityUnauthenticated =
        LoggerMessage.Define(
            LogLevel.Warning,
            LoggerEventIds.IdentityUnauthenticated,
            "User is not authenticated");

    private static readonly Action<ILogger, Exception?> IdentityClaimsExtracted =
        LoggerMessage.Define(
            LogLevel.Debug,
            LoggerEventIds.IdentityClaimsExtracted,
            "Claims extracted for user authentication");

    private static readonly Action<ILogger, string, Exception?> IdentityClaimsInvalid =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            LoggerEventIds.IdentityClaimsInvalid,
            "Invalid claims: {Error}");

    private static readonly Action<ILogger, string, Exception?> IdentityCreated =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.IdentityCreated,
            "User identity created for email {Email}");

    public static void LogIdentityUnauthenticated(this ILogger logger) =>
        IdentityUnauthenticated(logger, null);

    public static void LogIdentityClaimsExtracted(this ILogger logger) =>
        IdentityClaimsExtracted(logger, null);

    public static void LogIdentityClaimsInvalid(this ILogger logger, string error) =>
        IdentityClaimsInvalid(logger, error, null);

    public static void LogIdentityCreated(this ILogger logger, string email) =>
        IdentityCreated(logger, email, null);
}
