using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.ShoppingSession.Handlers;

public class ShoppingSessionCreatedHandler(ILogger<ShoppingSessionCreatedHandler> logger)
    : INotificationHandler<ShoppingSessionCreated>
{
    public Task Handle(ShoppingSessionCreated notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.ShoppingSession, nameof(notification.ShoppingSession));

        logger.LogDomainEvent(
            $"Shopping session converted to order - SessionId: {notification.ShoppingSession.Id}, " +
            $"UserId: {notification.ShoppingSession.UserId}"
        );

        return Task.CompletedTask;
    }
}
