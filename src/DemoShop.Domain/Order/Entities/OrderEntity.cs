using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Order.Enums;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Order.Entities;

public class OrderEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private readonly List<OrderItemEntity> _orderItems = [];

    public int Id { get; }
    public required int UserId { get; init; }
    public required OrderStatus Status { get; set; }
    public required Audit Audit { get; set; }
    public required SoftDeleteAudit SoftDeleteAudit { get; set; }
    public IReadOnlyCollection<OrderItemEntity> OrderItems => _orderItems;
    public User.Entities.UserEntity? User { get; init; }
    public void AddOrderItems(IEnumerable<OrderItemEntity> items) => _orderItems.AddRange(items);
}
