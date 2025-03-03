#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.User.Handlers;

public class UserAddressUpdatedHandler(ILogger logger)
    : INotificationHandler<UserAddressUpdatedDomainEvent>
{
    public Task Handle(UserAddressUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));
        Guard.Against.Null(notification.NewAddress, nameof(notification.NewAddress));

        LogUserAddressUpdated(logger, notification.Id);
        return Task.CompletedTask;
    }

    private static void LogUserAddressUpdated(ILogger logger, int id) => logger.Information(
        "User Address Updated: {Id} {@EventId}", id, LoggerEventIds.UserAddressUpdatedDomainEvent);
}
