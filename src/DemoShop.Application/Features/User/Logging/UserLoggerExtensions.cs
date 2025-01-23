using DemoShop.Domain.Common.Logging;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Logging;

public static class UserLoggerExtensions
{
    private static readonly Action<ILogger, string, Exception?> UserCreateStarted =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.AppUserCreateStarted,
            "Creating user with email {Email}");

    private static readonly Action<ILogger, string, Exception?> UserCreateValidationFailed =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            LoggerEventIds.AppUserCreateValidationFailed,
            "User creation validation failed: {Errors}");

    private static readonly Action<ILogger, string, Exception?> UserCreateFailed =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            LoggerEventIds.AppUserCreateFailed,
            "Failed to create user in repository for email {Email}");

    private static readonly Action<ILogger, string, Exception?> UserCreated =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.AppUserCreated,
            "User created successfully with ID {UserId}");

    private static readonly Action<ILogger, string, Exception?> UserGetOrCreateStarted =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.AppUserGetOrCreateStarted,
            "Getting or creating user for email {Email}");

    private static readonly Action<ILogger, string, Exception?> UserFound =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.AppUserFound,
            "Existing user found with ID {UserId}");

    private static readonly Action<ILogger, string, Exception?> UserNotFound =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.AppUserNotFound,
            "User not found, creating new user for email {Email}");

    private static readonly Action<ILogger, string, Exception?> UserDeleted =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.AppUserDeleted,
            "User with ID {UserId} deleted successfully ");

    private static readonly Action<ILogger, string, Exception?> UserRestored =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.AppUserRestored,
            "User with ID {UserId} restored successfully ");

    private static readonly Action<ILogger, string, Exception?> UserAddressUpdated =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.AppUserAddressUpdated,
            "Address for user with ID {UserId} updated successfully ");

    private static readonly Action<ILogger, string, Exception?> UserPhoneUpdated =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            LoggerEventIds.AppUserPhoneUpdated,
            "Phone number for user with ID {UserId} updated successfully ");

    public static void LogUserCreateStarted(this ILogger logger, string email) =>
        UserCreateStarted(logger, email, null);

    public static void LogUserCreateValidationFailed(this ILogger logger, string errors) =>
        UserCreateValidationFailed(logger, errors, null);

    public static void LogUserCreateFailed(this ILogger logger, string email) =>
        UserCreateFailed(logger, email, null);

    public static void LogUserCreated(this ILogger logger, string userId) =>
        UserCreated(logger, userId, null);

    public static void LogUserGetOrCreateStarted(this ILogger logger, string email) =>
        UserGetOrCreateStarted(logger, email, null);

    public static void LogUserFound(this ILogger logger, string userId) =>
        UserFound(logger, userId, null);

    public static void LogUserNotFound(this ILogger logger, string email) =>
        UserNotFound(logger, email, null);

    public static void LogUserDeleted(this ILogger logger, string userId) =>
        UserDeleted(logger, userId, null);

    public static void LogUserRestored(this ILogger logger, string userId) =>
        UserRestored(logger, userId, null);

    public static void LogUserAddressUpdated(this ILogger logger, string userId) =>
        UserAddressUpdated(logger, userId, null);

    public static void LogUserPhoneUpdated(this ILogger logger, string userId) =>
        UserPhoneUpdated(logger, userId, null);
}
