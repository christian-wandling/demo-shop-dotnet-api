#region

using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Order.Entities;

#endregion

namespace DemoShop.Domain.Order.Events;

public class OrderCreatedDomainEvent(OrderEntity order) : IDomainEvent
{
    public OrderEntity Order { get; } = order;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
