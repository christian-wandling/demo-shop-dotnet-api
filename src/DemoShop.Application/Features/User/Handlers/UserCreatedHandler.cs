#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Handlers;

public class UserCreatedHandler(ILogger logger)
    : INotificationHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.User, nameof(notification.User));

        LogUserCreated(logger, notification.User.Id);
        return Task.CompletedTask;
    }

    private static void LogUserCreated(ILogger logger, int id) => logger.Information(
        "User Created: {Id} {@EventId}", id, LoggerEventIds.UserCreatedDomainEvent);
}
