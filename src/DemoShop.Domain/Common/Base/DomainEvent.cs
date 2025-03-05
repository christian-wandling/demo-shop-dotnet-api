namespace DemoShop.Domain.Common.Base;

public abstract record DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
