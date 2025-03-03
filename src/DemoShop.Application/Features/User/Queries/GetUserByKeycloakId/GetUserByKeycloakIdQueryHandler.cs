#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Interfaces;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;

public sealed class GetUserByKeycloakIdQueryHandler(
    IMapper mapper,
    IUserRepository repository,
    ILogger logger,
    ICacheService cacheService
)
    : IRequestHandler<GetUserByKeycloakIdQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(GetUserByKeycloakIdQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        try
        {
            LogQueryStarted(logger, request.KeycloakUserId);

            var cacheKey = cacheService.GenerateCacheKey("user", request);
            var response = cacheService.GetFromCache<UserResponse>(cacheKey)
                           ?? await GetFromDatabase(request.KeycloakUserId, cacheKey, cancellationToken);

            if (response is null)
            {
                LogNotFound(logger, request.KeycloakUserId);
                return Result.NotFound("User not found");
            }

            LogQuerySuccess(logger, response.Id);
            return Result.Success(response);
        }
        catch (InvalidOperationException ex)
        {
            LogInvalidOperationException(logger, request.KeycloakUserId, ex.Message, ex);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            LogDatabaseException(logger, request.KeycloakUserId, ex.Message, ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<UserResponse?> GetFromDatabase(
        string keycloakUserId, string cacheKey, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByKeycloakIdAsync(keycloakUserId, cancellationToken);

        if (user is null)
            return null;

        var response = mapper.Map<UserResponse>(user);
        cacheService.SetCache(cacheKey, response);

        return response;
    }

    private static void LogQueryStarted(ILogger logger, string keycloakUserId) =>
        logger.ForContext("EventId", LoggerEventIds.GetUserByKeycloakIdQueryStarted)
            .Information("Starting query to retrieve user with KeycloakUserId {KeycloakUserId}", keycloakUserId);

    private static void LogQuerySuccess(ILogger logger, int userId) =>
        logger.ForContext("EventId", LoggerEventIds.GetUserByKeycloakIdQuerySuccess)
            .Information("Successfully retrieved user with ID {UserId}", userId);

    private static void LogNotFound(ILogger logger, string keycloakUserId) =>
        logger.ForContext("EventId", LoggerEventIds.GetUserByKeycloakIdQueryNotFound)
            .Information("User with KeycloakUserId {KeycloakUserId} was not found", keycloakUserId);

    private static void
        LogDatabaseException(ILogger logger, string keycloakUserId, string errorMessage, Exception ex) =>
        logger.ForContext("EventId", LoggerEventIds.GetUserByKeycloakIdDatabaseException)
            .Error(ex, "Database error occurred while retrieving user with ID {KeycloakUserId}. Error: {ErrorMessage}",
                keycloakUserId, errorMessage);

    private static void
        LogInvalidOperationException(ILogger logger, string keycloakUserId, string errorMessage, Exception ex) =>
        logger.ForContext("EventId", LoggerEventIds.GetUserByKeycloakIdDomainException)
            .Error(ex, "Invalid operation while retrieving user with ID {KeycloakUserId}. Error: {ErrorMessage}",
                keycloakUserId, errorMessage);
}
