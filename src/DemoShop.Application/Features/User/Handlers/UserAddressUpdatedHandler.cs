#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.User.Handlers;

public class UserAddressUpdatedHandler(ILogger<UserAddressUpdatedHandler> logger)
    : INotificationHandler<UserAddressUpdatedDomainEvent>
{
    public Task Handle(UserAddressUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));
        Guard.Against.Null(notification.NewAddress, nameof(notification.NewAddress));

        logger.LogOperationSuccess("Update user address", "id", $"{notification.Id}");
        return Task.CompletedTask;
    }
}
