using Ardalis.GuardClauses;
using DemoShop.Application.Features.User.Logging;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Events;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Events;

public class UserCreatedHandler(ILogger<UserCreatedHandler> logger)
    : IHandle<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Guard.Against.Null(domainEvent, nameof(domainEvent));
        Guard.Against.Null(domainEvent.User, nameof(domainEvent.User));

        logger.LogUserCreated($"{domainEvent.User.Id}");
        return Task.CompletedTask;
    }
}
