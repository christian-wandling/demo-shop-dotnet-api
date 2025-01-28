using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.Entities;

namespace DemoShop.Domain.Product.Events;

// TODO dry?
public class CategoryCreatedDomainEvent(CategoryEntity category) : IDomainEvent
{
    public CategoryEntity Category { get; } = category;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
