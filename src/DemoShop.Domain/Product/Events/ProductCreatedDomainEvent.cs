#region

using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.Entities;

#endregion

namespace DemoShop.Domain.Product.Events;

public class ProductCreatedDomainEvent(ProductEntity product) : IDomainEvent
{
    public ProductEntity Product { get; } = product;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
