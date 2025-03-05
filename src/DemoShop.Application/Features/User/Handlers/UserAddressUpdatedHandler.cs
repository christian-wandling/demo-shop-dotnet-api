#region

using Ardalis.GuardClauses;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Handlers;

public class UserAddressUpdatedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<UserAddressUpdatedDomainEvent>
{
    public Task Handle(UserAddressUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));
        Guard.Against.Null(notification.NewAddress, nameof(notification.NewAddress));

        InvalidateCache(notification.Id, notification.KeycloakUserId);
        LogUserAddressUpdated(logger, notification.Id);
        return Task.CompletedTask;
    }

    private void InvalidateCache(int id, string keycloakUserId)
    {
        var cacheKeyUser = cacheService.GenerateCacheKey("user", new GetUserByKeycloakIdQuery(keycloakUserId));
        cacheService.InvalidateCache(cacheKeyUser);
        var cacheKeyordersOfUser = cacheService.GenerateCacheKey("orders-of-user", id);
        cacheService.InvalidateCache(cacheKeyordersOfUser);
    }

    private static void LogUserAddressUpdated(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.UserAddressUpdatedDomainEvent)
        .Information("User Address Updated: {Id}", id);
}
