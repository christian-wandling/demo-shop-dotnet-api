using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Domain.Product.Events;

// TODO dry?
public class CategoryRestoredDomainEvent(int categoryId) : IDomainEvent
{
    public int Id { get; } = categoryId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
