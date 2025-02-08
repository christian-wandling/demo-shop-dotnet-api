#region

using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Order.Events;

public class OrderRestoredDomainEvent(int orderId) : IDomainEvent
{
    public int Id { get; } = orderId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
