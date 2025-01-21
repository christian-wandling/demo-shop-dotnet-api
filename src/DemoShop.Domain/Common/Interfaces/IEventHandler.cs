namespace DemoShop.Domain.Common.Interfaces;

public interface IHandle<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken = default);
}
