using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.ShoppingSession.Handlers;

public class CartItemRemovedHandler(ILogger<CartItemRemovedHandler> logger)
    : INotificationHandler<CartItemRemoved>
{
    public Task Handle(CartItemRemoved notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.CartItem, nameof(notification.CartItem));

        logger.LogDomainEvent(
            $"Cart item removed - CartItemId: {notification.CartItem.Id}, " +
            $"Product: {notification.CartItem.Product?.Name}, " +
            $"SessionId: {notification.CartItem.ShoppingSessionId}, " +
            $"UserId: {notification.CartItem.ShoppingSession?.UserId}"
        );

        return Task.CompletedTask;
    }
}
