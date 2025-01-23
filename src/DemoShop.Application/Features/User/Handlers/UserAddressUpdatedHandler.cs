using Ardalis.GuardClauses;
using DemoShop.Application.Features.User.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Handlers;

public class UserPhoneUpdatedHandler(ILogger<UserPhoneUpdatedHandler> logger)
    : INotificationHandler<UserPhoneUpdatedDomainEvent>
{
    public Task Handle(UserPhoneUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));
        Guard.Against.Null(notification.NewPhone, nameof(notification.NewPhone));

        logger.LogUserPhoneUpdated($"{notification.Id}");
        return Task.CompletedTask;
    }
}
