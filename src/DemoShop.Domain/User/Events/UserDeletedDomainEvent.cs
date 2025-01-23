using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Domain.User.Events;

public class UserDeletedDomainEvent(int id) : IDomainEvent
{
    public int Id { get; } = id;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
