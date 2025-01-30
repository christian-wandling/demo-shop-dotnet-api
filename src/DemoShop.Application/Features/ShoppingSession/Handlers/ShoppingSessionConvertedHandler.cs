using Ardalis.GuardClauses;
using DemoShop.Application.Features.User.Handlers;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.ShoppingSession.Handlers;

public class ShoppingSessionConvertedHandler(ILogger<ShoppingSessionConvertedHandler> logger)
    : INotificationHandler<ShoppingSessionConverted>
{
    public Task Handle(ShoppingSessionConverted notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.ShoppingSession, nameof(notification.ShoppingSession));
        Guard.Against.NegativeOrZero(notification.OrderId, nameof(notification.OrderId));

        logger.LogDomainEvent(
            $"Shopping session converted to order - SessionId: {notification.ShoppingSession.Id}, " +
            $"OrderId: {notification.OrderId}, UserId: {notification.ShoppingSession.UserId}"
        );

        return Task.CompletedTask;
    }
}
