#region

using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Product.Events;

public class ImageRestoredDomainEvent(int imageId) : IDomainEvent
{
    public int Id { get; } = imageId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
