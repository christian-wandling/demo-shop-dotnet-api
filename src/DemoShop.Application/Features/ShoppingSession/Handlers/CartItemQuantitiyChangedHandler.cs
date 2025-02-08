#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Events;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Handlers;

public class CartItemQuantityChangedHandler(ILogger<CartItemQuantityChangedHandler> logger)
    : INotificationHandler<CartItemQuantityChanged>
{
    public Task Handle(CartItemQuantityChanged notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.CartItem, nameof(notification.CartItem));
        Guard.Against.NegativeOrZero(notification.OldQuantity, nameof(notification.OldQuantity));

        logger.LogDomainEvent(
            $"Cart item quantitiy changed - CartItemId: {notification.CartItem.Id}, " +
            $"Product: {notification.CartItem.Product?.Name}, " +
            $"Quantity: {notification.OldQuantity} to {notification.CartItem.Quantity}, " +
            $"SessionId: {notification.CartItem.ShoppingSessionId}, " +
            $"UserId: {notification.CartItem.ShoppingSession?.UserId}"
        );

        return Task.CompletedTask;
    }
}
