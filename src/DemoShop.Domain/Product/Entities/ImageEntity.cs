using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Product.Entities;

public class ImageEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    public int Id { get; }
    public required string Name { get; set; }
    public required Uri Uri { get; set; }
    public int? ProductId { get; init; }

    public ProductEntity? Product { get; init; }
    public required Audit Audit { get; set; }
    public required SoftDeleteAudit SoftDeleteAudit { get; set; }
}
