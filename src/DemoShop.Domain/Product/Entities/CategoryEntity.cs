using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Product.Entities;

public class CategoryEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    public int Id { get; }
    public required string Name { get; set; }

    public ICollection<ProductEntity> Products { get; } = new List<ProductEntity>();
    public required Audit Audit { get; set; }
    public required SoftDeleteAudit SoftDeleteAudit { get; set; }
}
