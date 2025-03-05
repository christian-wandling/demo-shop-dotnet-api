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

public class UserPhoneUpdatedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<UserPhoneUpdatedDomainEvent>
{
    public Task Handle(UserPhoneUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));
        Guard.Against.Null(notification.NewPhone, nameof(notification.NewPhone));

        InvalidateCache(notification.KeycloakUserId);
        LogUserPhoneUpdated(logger, notification.Id);
        return Task.CompletedTask;
    }

    private void InvalidateCache(string keycloakUserId)
    {
        var cacheKeyUser = cacheService.GenerateCacheKey("user", new GetUserByKeycloakIdQuery(keycloakUserId));
        cacheService.InvalidateCache(cacheKeyUser);
    }

    private static void LogUserPhoneUpdated(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.UserPhoneUpdatedDomainEvent)
        .Information("User Phone Updated: {Id}", id);
}
