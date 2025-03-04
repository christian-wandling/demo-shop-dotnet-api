#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using DemoShop.Domain.User.ValueObjects;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Features.Users;

public class UserRepository(ApplicationDbContext context, ILogger logger) : IUserRepository
{
    public async Task<UserEntity?> GetUserByKeycloakIdAsync(string value, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));
        LogGetUserByKeycloakUserIdStarted(logger, value);

        var keycloakUserId = KeycloakUserId.Create(value);

        var result = await context.Query<UserEntity>()
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.KeycloakUserId.Equals(keycloakUserId), cancellationToken);

        if (result is null)
            LogGetUserByKeycloakUserIdNotFound(logger, value);
        else
            LogGetUserByKeycloakUserIdSuccess(logger, value);

        return result;
    }

    public async Task<UserEntity?> CreateUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
        Guard.Against.Null(user, nameof(user));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));
        LogCreateUserStarted(logger, user.KeycloakUserId.Value);

        var entry = await context.Set<UserEntity>().AddAsync(user, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        LogCreateUserSuccess(logger, user.Id, user.KeycloakUserId.Value);
        return entry.Entity;
    }

    public async Task<UserEntity> UpdateUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
        Guard.Against.Null(user, nameof(user));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));
        LogUpdateUserStarted(logger, user.Id);

        var entry = context.Set<UserEntity>().Update(user);
        await context.SaveChangesAsync(cancellationToken);
        await entry.ReloadAsync(cancellationToken);

        LogUpdateUserSuccess(logger, entry.Entity.Id);
        return entry.Entity;
    }

    private static void LogGetUserByKeycloakUserIdStarted(ILogger logger, string keycloakUserId) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetUserByKeycloakUserIdStarted)
            .Debug("Attempting to get user with KeycloakUserId {KeycloakUserId}", keycloakUserId);

    private static void LogGetUserByKeycloakUserIdSuccess(ILogger logger, string keycloakUserId) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetUserByKeycloakUserIdSuccess)
            .Debug("Attempting to get user with KeycloakUserId {KeycloakUserId} completed successfully",
                keycloakUserId);

    private static void LogGetUserByKeycloakUserIdNotFound(ILogger logger, string keycloakUserId) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetUserByKeycloakUserIdNotFound)
            .Debug("User with KeycloakUserId {KeycloakUserId} not found in database", keycloakUserId);

    private static void LogCreateUserStarted(ILogger logger, string keycloakUserId) =>
        logger
            .ForContext("EventId", LoggerEventIds.CreateUserStarted)
            .Debug("Attempting to create user for KeycloakUserId {KeycloakUserId}", keycloakUserId);

    private static void LogCreateUserSuccess(ILogger logger, int userId, string keycloakUserId) =>
        logger
            .ForContext("EventId", LoggerEventIds.CreateUserSuccess)
            .Debug("Successfully created user with Id {UserId} for KeycloakUserId {KeycloakUserId}",
                userId, keycloakUserId);

    private static void LogUpdateUserStarted(ILogger logger, int userId) =>
        logger
            .ForContext("EventId", LoggerEventIds.UpdateUserStarted)
            .Debug("Attempting to update user with userId {UserId}", userId);

    private static void LogUpdateUserSuccess(ILogger logger, int userId) =>
        logger
            .ForContext("EventId", LoggerEventIds.UpdateUserSuccess)
            .Debug("Successfully updated user with Id {UserId}", userId);
}
