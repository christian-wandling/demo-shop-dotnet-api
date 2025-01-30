using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.ShoppingSession.Handlers;

public class CartItemAddedHandler(ILogger<CartItemAddedHandler> logger)
    : INotificationHandler<CartItemAdded>
{
    public Task Handle(CartItemAdded notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.CartItem, nameof(notification.CartItem));

        logger.LogDomainEvent(
            $"Cart item added - CartItemId: {notification.CartItem.Id}, " +
            $"Product: {notification.CartItem.Product?.Name}, " +
            $"Quantity: {notification.CartItem.Quantity}, " +
            $"SessionId: {notification.CartItem.ShoppingSessionId}, " +
            $"UserId: {notification.CartItem.ShoppingSession?.UserId}"
        );

        return Task.CompletedTask;
    }
}
