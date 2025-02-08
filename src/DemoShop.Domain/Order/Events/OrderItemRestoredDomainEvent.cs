#region

using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Order.Events;

public class OrderItemRestoredDomainEvent(int orderItemId) : IDomainEvent
{
    public int Id { get; } = orderItemId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
