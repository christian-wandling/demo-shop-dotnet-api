#region

using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.Entities;

#endregion

namespace DemoShop.Domain.Product.Events;

// TODO dry?
public class ImageCreatedDomainEvent(ImageEntity image) : IDomainEvent
{
    public ImageEntity Image { get; } = image;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
