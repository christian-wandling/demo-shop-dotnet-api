#region

using Ardalis.GuardClauses;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Handlers;

public class ShoppingSessionConvertedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<ShoppingSessionConverted>
{
    public Task Handle(ShoppingSessionConverted notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.ShoppingSession, nameof(notification.ShoppingSession));

        InvalidateCache(notification.ShoppingSession.UserId);
        LogShoppingSessionConverted(logger, notification.ShoppingSession.Id);

        return Task.CompletedTask;
    }

    private void InvalidateCache(int userId)
    {
        List<string> cacheKeys =
        [
            cacheService.GenerateCacheKey("shoppingSession", new GetShoppingSessionByUserIdQuery(userId)),
            cacheService.GenerateCacheKey("orders-of-user", userId),
            cacheService.GenerateCacheKey("current-session-accessor", userId),
        ];

        cacheKeys.ForEach(cacheService.InvalidateCache);
    }

    private static void LogShoppingSessionConverted(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.ShoppingSessionConvertedDomainEvent)
        .Information("ShoppingSession Converted: {Id}", id);
}
