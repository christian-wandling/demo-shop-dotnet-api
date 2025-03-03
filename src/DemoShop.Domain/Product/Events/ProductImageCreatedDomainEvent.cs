#region

using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.Entities;

#endregion

namespace DemoShop.Domain.Product.Events;

public class ProductImageCreatedDomainEvent(ImageEntity image) : IDomainEvent
{
    public ImageEntity Image { get; } = image;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
