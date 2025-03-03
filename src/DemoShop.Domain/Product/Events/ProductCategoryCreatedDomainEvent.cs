#region

using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.Entities;

#endregion

namespace DemoShop.Domain.Product.Events;

public class ProductCategoryCreatedDomainEvent(CategoryEntity category) : IDomainEvent
{
    public CategoryEntity Category { get; } = category;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
