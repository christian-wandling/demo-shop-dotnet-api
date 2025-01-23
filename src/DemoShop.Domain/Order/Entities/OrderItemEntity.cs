using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Order.Entities;

public class OrderItemEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    public int Id { get; }
    public int OrderId { get; init; }
    public required int ProductId { get; init; }
    public required string ProductName { get; init; }
    public required Uri ProductThumbnail { get; init; }
    public required int Quantity { get; set; }
    public required decimal Price { get; init; }

    public OrderEntity? Order { get; init; }
    public required Audit Audit { get; set; }
    public required SoftDeleteAudit SoftDeleteAudit { get; set; }
}
