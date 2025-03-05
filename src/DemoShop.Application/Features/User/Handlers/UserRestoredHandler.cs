#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Handlers;

public class UserRestoredHandler(ILogger logger)
    : INotificationHandler<UserRestoredDomainEvent>
{
    public Task Handle(UserRestoredDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        LogUserRestored(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogUserRestored(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.UserRestoredDomainEvent)
        .Information("User Restored: {Id}", id);
}
