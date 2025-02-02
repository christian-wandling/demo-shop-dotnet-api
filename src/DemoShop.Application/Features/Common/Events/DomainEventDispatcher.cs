using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using MediatR;

namespace DemoShop.Application.Features.Common.Events;

public class DomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchEventsAsync(IEntity entity, CancellationToken cancellationToken)
    {
        var events = entity.GetDomainEvents();

        foreach (var domainEvent in events)
            await mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);

        entity.ClearDomainEvents();
    }
}
