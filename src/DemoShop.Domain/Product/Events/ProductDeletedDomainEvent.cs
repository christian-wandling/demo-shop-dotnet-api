#region

using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Product.Events;

public class ProductDeletedDomainEvent(int id) : IDomainEvent
{
    public int Id { get; } = id;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
