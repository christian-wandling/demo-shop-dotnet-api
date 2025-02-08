#region

using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Product.Events;

// TODO dry?
public class ImageDeletedDomainEvent(int imageId) : IDomainEvent
{
    public int Id { get; } = imageId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
