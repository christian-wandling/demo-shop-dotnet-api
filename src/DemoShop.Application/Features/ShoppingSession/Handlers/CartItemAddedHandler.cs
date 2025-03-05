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

public class CartItemAddedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<CartItemAdded>
{
    public Task Handle(CartItemAdded notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        LogCartItemAdded(logger, notification.Id);
        InvalidateCache(notification.UserId);
        return Task.CompletedTask;
    }

    private void InvalidateCache(int userId)
    {
        List<string> cacheKeys =
        [
            cacheService.GenerateCacheKey("user", new GetShoppingSessionByUserIdQuery(userId)),
            cacheService.GenerateCacheKey("current-session-accessor", userId)
        ];

        cacheKeys.ForEach(cacheService.InvalidateCache);
    }

    private static void LogCartItemAdded(ILogger logger, int id) => logger.Information(
        "CartItem Added: {Id} {@EventId}", id, LoggerEventIds.CartItemAddedDomainEvent);
}
