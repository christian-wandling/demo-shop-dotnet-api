#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Entities;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Processes.UserResolution;

public sealed class UserResolutionProcessHandler(
    IUserIdentityAccessor identity,
    IMediator mediator,
    ILogger logger
)
    : IRequestHandler<UserResolutionProcess, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(UserResolutionProcess request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var identityResult = identity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
            return identityResult.Map<IUserIdentity, UserResponse>(null);

        LogProcessStarted(logger, identityResult.Value.KeycloakUserId);

        var result = await GetUserByKeycloakId(identityResult.Value.KeycloakUserId, cancellationToken);
        if (!result.IsSuccess)
            result = await CreateUser(identityResult.Value, cancellationToken);

        if (result.IsSuccess)
            LogProcessSuccess(logger, result.Value.Id, identityResult.Value.KeycloakUserId);
        else
            LogProcessFailed(logger, identityResult.Value.KeycloakUserId);

        return result;
    }

    private async Task<Result<UserResponse>> GetUserByKeycloakId(string keycloakUserId,
        CancellationToken cancellationToken)
    {
        var command = new GetUserByKeycloakIdQuery(keycloakUserId);
        return await mediator.Send(command, cancellationToken);
    }

    private async Task<Result<UserResponse>> CreateUser(IUserIdentity userIdentity, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(userIdentity);
        return await mediator.Send(command, cancellationToken);
    }

    private static void LogProcessStarted(ILogger logger, string keycloakUserId) =>
        logger.ForContext("EventId", LoggerEventIds.UserResolutionProcessStarted)
            .Information("Resolving user for keycloakUserId {KeycloakUserId}", keycloakUserId);

    private static void LogProcessSuccess(ILogger logger, int userId, string keycloakUserId) =>
        logger.ForContext("EventId", LoggerEventIds.UserResolutionProcessSuccess)
            .Information(
                "Successfully resolved user with Id {UserId} for KeycloakUserId {KeycloakUserId}",
                userId, keycloakUserId);

    private static void LogProcessFailed(ILogger logger, string keycloakUserId) =>
        logger.ForContext("EventId", LoggerEventIds.UserResolutionProcessFailed)
            .Information("Error while resolving user for keycloakUserId {KeycloakUserId}", keycloakUserId);
}
