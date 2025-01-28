using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.Entities;

namespace DemoShop.Domain.Product.Events;

// TODO Dry?

public class ProductCreatedDomainEvent(ProductEntity product) : IDomainEvent
{
    public ProductEntity Product { get; } = product;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
