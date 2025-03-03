#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Handlers;

public class UserDeletedHandler(ILogger logger)
    : INotificationHandler<UserDeletedDomainEvent>
{
    public Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogUserDeleted(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogUserDeleted(ILogger logger, int id) => logger.Information(
        "User Deleted: {Id} {@EventId}", id, LoggerEventIds.UserDeletedDomainEvent);
}
