#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Handlers;

public class UserPhoneUpdatedHandler(ILogger logger)
    : INotificationHandler<UserPhoneUpdatedDomainEvent>
{
    public Task Handle(UserPhoneUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));
        Guard.Against.Null(notification.NewPhone, nameof(notification.NewPhone));

        LogUserPhoneUpdated(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogUserPhoneUpdated(ILogger logger, int id) => logger.Information(
        "User Phone Updated: {Id} {@EventId}", id, LoggerEventIds.UserPhoneUpdatedDomainEvent);
}
