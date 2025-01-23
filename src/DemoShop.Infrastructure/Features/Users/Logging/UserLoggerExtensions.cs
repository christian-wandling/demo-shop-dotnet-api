using DemoShop.Domain.Common.Logging;
using Microsoft.Extensions.Logging;

namespace DemoShop.Infrastructure.Features.Users.Logging;

public static class UserLoggerExtensions
{
    private static readonly Action<ILogger, string, Exception?> GetUserByIdStarted =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.InfraGetUserByIdStarted,
            "Getting user with id {Id}");

    private static readonly Action<ILogger, string, Exception?> GetUserByIdSuccess =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.InfraGetUserByIdSuccess,
            "Retrieved user with id {Id}");

    private static readonly Action<ILogger, string, Exception?> GetUserByIdFailed =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.InfraGetUserByIdFailed,
            "Error getting user with id {Id}");

    private static readonly Action<ILogger, string, Exception?> GetUserByEmailStarted =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.InfraGetUserByEmailStarted,
            "Getting user with email {Email}");

    private static readonly Action<ILogger, string, Exception?> GetUserByEmailSuccess =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.InfraGetUserByEmailSuccess,
            "Retrieved user with email {Email}");

    private static readonly Action<ILogger, string, Exception?> GetUserByEmailFailed =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.InfraGetUserByEmailFailed,
            "Error getting user with email {Email}");

    private static readonly Action<ILogger, string, Exception?> CreateUserStarted =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.InfraCreateUserStarted,
            "Creating user with email {Email}");

    private static readonly Action<ILogger, string, Exception?> CreateUserSuccess =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.InfraCreateUserSuccess,
            "Created user with id {Id}");

    private static readonly Action<ILogger, string, Exception?> CreateUserFailed =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.InfraCreateUserFailed,
            "Error creating user with email {Email}");

    public static void LogCreateUserStarted(this ILogger logger, string email) =>
        CreateUserStarted(logger, email, null);

    public static void LogCreateUserSuccess(this ILogger logger, string email) =>
        CreateUserSuccess(logger, email, null);

    public static void LogCreateUserFailed(this ILogger logger, string email, Exception ex) =>
        CreateUserFailed(logger, email, ex);

    public static void LogGetUserByIdStarted(this ILogger logger, string id) =>
        GetUserByIdStarted(logger, id, null);

    public static void LogGetUserByIdSuccess(this ILogger logger, string id) =>
        GetUserByIdSuccess(logger, id, null);

    public static void LogGetUserByIdFailed(this ILogger logger, string id, Exception ex) =>
        GetUserByIdFailed(logger, id, ex);

    public static void LogGetUserByEmailStarted(this ILogger logger, string email) =>
        GetUserByEmailStarted(logger, email, null);

    public static void LogGetUserByEmailSuccess(this ILogger logger, string email) =>
        GetUserByEmailSuccess(logger, email, null);

    public static void LogGetUserByEmailFailed(this ILogger logger, string email, Exception ex) =>
        GetUserByEmailFailed(logger, email, ex);
}
