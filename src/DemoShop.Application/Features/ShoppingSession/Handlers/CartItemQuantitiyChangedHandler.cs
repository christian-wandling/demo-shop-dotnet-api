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

public class CartItemQuantityChangedHandler(ILogger logger, ICacheService cacheService)
    : INotificationHandler<CartItemQuantityChanged>
{
    public Task Handle(CartItemQuantityChanged notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.CartItem, nameof(notification.CartItem));
        Guard.Against.NegativeOrZero(notification.OldQuantity, nameof(notification.OldQuantity));

        LogCartItemQuantityChanged(logger, notification.CartItem.Id);
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

    private static void LogCartItemQuantityChanged(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.CartItemQuantityChangedDomainEvent)
        .Information(
        "CartItem quantity changed: {Id}", id);
}
