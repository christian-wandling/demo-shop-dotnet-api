#region

using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.Domain.User.Entities;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Features.ShoppingSessions.Services;

public sealed class CurrentShoppingSessionAccessor(
    IShoppingSessionRepository repository,
    ICurrentUserAccessor currentUser,
    ILogger logger
)
    : ICurrentShoppingSessionAccessor
{
    public async Task<Result<ShoppingSessionEntity>> GetCurrent(CancellationToken cancellationToken)
    {
        var userIdResult = await currentUser.GetId(cancellationToken);

        if (!userIdResult.IsSuccess)
            return userIdResult.Map();

        var session = await repository.GetSessionByUserIdAsync(userIdResult.Value, cancellationToken);

        if (session is null)
        {
            LogNotFound(logger, userIdResult.Value);
            return Result.NotFound("No active shopping session session found for current user");
        }

        LogSuccess(logger, session);
        return Result.Success(session);
    }

    private static void LogSuccess(ILogger logger, ShoppingSessionEntity session) =>
        logger.Information(
            "[{EventId}] Shopping session with id {Id} found for user {UserId}",
            LoggerEventIds.CurrentUserAccessorSuccess,
            session.Id, session.UserId);

    private static void LogNotFound(ILogger logger, int userId) =>
        logger.Error(
            "[{EventId}] No shopping session found for user {UserId}",
            LoggerEventIds.CurrentUserAccessorNotFound,
            userId);
}
