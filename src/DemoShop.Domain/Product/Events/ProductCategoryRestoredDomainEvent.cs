#region

using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Product.Events;

public class ProductCategoryRestoredDomainEvent(int categoryId) : IDomainEvent
{
    public int Id { get; } = categoryId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
