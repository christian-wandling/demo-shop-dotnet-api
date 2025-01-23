using Ardalis.GuardClauses;
using DemoShop.Application.Features.User.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Handlers;

public class UserAddressUpdatedHandler(ILogger<UserAddressUpdatedHandler> logger)
    : INotificationHandler<UserAddressUpdatedDomainEvent>
{
    public Task Handle(UserAddressUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));
        Guard.Against.Null(notification.NewAddress, nameof(notification.NewAddress));

        logger.LogUserAddressUpdated($"{notification.Id}");
        return Task.CompletedTask;
    }
}
