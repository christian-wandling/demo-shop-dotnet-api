using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Domain.Product.Events;

// TODO dry?
public class ProductDeletedDomainEvent(int id) : IDomainEvent
{
    public int Id { get; } = id;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
