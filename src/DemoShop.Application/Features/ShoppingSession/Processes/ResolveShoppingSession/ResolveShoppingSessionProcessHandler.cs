#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Processes.ResolveShoppingSession;

public sealed class ResolveShoppingSessionProcessHandler(
    ICurrentUserAccessor user,
    IMediator mediator,
    ILogger logger
)
    : IRequestHandler<ResolveShoppingSessionProcess, Result<ShoppingSessionResponse>>
{
    public async Task<Result<ShoppingSessionResponse>> Handle(ResolveShoppingSessionProcess request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var userIdResult = await user.GetId(cancellationToken);

        if (!userIdResult.IsSuccess)
            return userIdResult.Map();

        LogProcessStarted(logger, userIdResult.Value);

        var result = await GetShoppingSessionByUserId(userIdResult.Value, cancellationToken);
        if (!result.IsSuccess)
            result = await CreateShoppingSession(userIdResult.Value, cancellationToken);

        if (result.IsSuccess)
            LogProcessSuccess(logger, result.Value.Id, userIdResult.Value);
        else
            LogProcessFailed(logger, userIdResult.Value);

        return result;
    }

    private async Task<Result<ShoppingSessionResponse>> GetShoppingSessionByUserId(int userId,
        CancellationToken cancellationToken)
    {
        var command = new GetShoppingSessionByUserIdQuery(userId);
        return await mediator.Send(command, cancellationToken);
    }

    private async Task<Result<ShoppingSessionResponse>> CreateShoppingSession(int userId,
        CancellationToken cancellationToken)
    {
        var command = new CreateShoppingSessionCommand(userId);
        return await mediator.Send(command, cancellationToken);
    }

    private static void LogProcessStarted(ILogger logger, int userId) =>
        logger.ForContext("EventId", LoggerEventIds.ResolveShoppingSessionProcessStarted)
            .Information("Resolving shopping session for UserId {UserId}", userId);

    private static void LogProcessSuccess(ILogger logger, int sessionId, int userId) =>
        logger.ForContext("EventId", LoggerEventIds.ResolveShoppingSessionProcessSuccess)
            .Information(
                "Successfully resolved shopping session with Id {UserId} for UserId {UserId}",
                sessionId, userId);

    private static void LogProcessFailed(ILogger logger, int userId) =>
        logger.ForContext("EventId", LoggerEventIds.ResolveShoppingSessionProcessFailed)
            .Information("Error while resolving shopping session for UserId {UserId}", userId);
}
