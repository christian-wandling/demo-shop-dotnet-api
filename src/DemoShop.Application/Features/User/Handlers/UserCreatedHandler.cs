using Ardalis.GuardClauses;
using DemoShop.Application.Features.User.Logging;
using DemoShop.Domain.User.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Handlers;

public class UserCreatedHandler(ILogger<UserCreatedHandler> logger)
    : INotificationHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.User, nameof(notification.User));

        logger.LogUserCreated($"{notification.User.Id}");
        return Task.CompletedTask;
    }
}
