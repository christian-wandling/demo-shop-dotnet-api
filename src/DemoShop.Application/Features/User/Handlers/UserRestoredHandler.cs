using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Handlers;

public class UserRestoredHandler(ILogger<UserRestoredHandler> logger)
    : INotificationHandler<UserRestoredDomainEvent>
{
    public Task Handle(UserRestoredDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        logger.LogOperationSuccess("Restore User", "id", $"{notification.Id}");
        return Task.CompletedTask;
    }
}
