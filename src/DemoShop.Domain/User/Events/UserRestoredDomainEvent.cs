using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Domain.User.Events;

public class UserRestoredDomainEvent(int id) : IDomainEvent
{
    public int Id { get; } = id;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
