namespace DemoShop.Domain.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEntity entity, CancellationToken cancellationToken);
}
