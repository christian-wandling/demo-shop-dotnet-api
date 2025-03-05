#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Handlers;

public class ShoppingSessionCreatedHandler(ILogger logger)
    : INotificationHandler<ShoppingSessionCreated>
{
    public Task Handle(ShoppingSessionCreated notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.ShoppingSession, nameof(notification.ShoppingSession));

        LogShoppingSessionCreated(logger, notification.ShoppingSession.Id);

        return Task.CompletedTask;
    }

    private static void LogShoppingSessionCreated(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.ShoppingSessionCreatedDomainEvent)
        .Information("ShoppingSession Created: {Id}", id);
}
