#region

using Ardalis.GuardClauses;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Handlers;

public class UserDeletedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<UserDeletedDomainEvent>
{
    public Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        InvalidateCache(notification.Id, notification.KeycloakUserId);
        LogUserDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private void InvalidateCache(int id, string keycloakUserId)
    {
        List<string> cacheKeys =
        [
            cacheService.GenerateCacheKey("user", new GetUserByKeycloakIdQuery(keycloakUserId)),
            cacheService.GenerateCacheKey("user", new GetShoppingSessionByUserIdQuery(id)),
            cacheService.GenerateCacheKey("orders-of-user", id),
            cacheService.GenerateCacheKey("current-user-accessor", keycloakUserId),
            cacheService.GenerateCacheKey("current-session-accessor", id)
        ];

        cacheKeys.ForEach(cacheService.InvalidateCache);
    }

    private static void LogUserDeleted(ILogger logger, int id) => logger.Information(
        "User Deleted: {Id} {@EventId}", id, LoggerEventIds.UserDeletedDomainEvent);
}
