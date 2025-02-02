using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Domain.Order.Events;

public class OrderItemDeletedDomainEvent(int orderItemId) : IDomainEvent
{
    public int Id { get; } = orderItemId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
