#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.Enums;
using DemoShop.Domain.Order.Events;
using DemoShop.Domain.User.Entities;

#endregion

namespace DemoShop.Domain.Order.Entities;

public sealed class OrderEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private readonly List<OrderItemEntity> _orderItems = [];

    private OrderEntity()
    {
        Status = OrderStatus.Created;
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    private OrderEntity(int userId, IReadOnlyCollection<OrderItemEntity> items)
    {
        UserId = Guard.Against.NegativeOrZero(userId, nameof(userId));
        Status = OrderStatus.Created;
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();

        Guard.Against.Null(items, nameof(items));
        Guard.Against.NegativeOrZero(items.Count, nameof(items.Count));
        _orderItems.AddRange(items);
    }

    public OrderStatus Status { get; private set; }
    public int UserId { get; init; }
    public UserEntity? User { get; init; }

    public IReadOnlyCollection<OrderItemEntity> OrderItems => _orderItems;

    public Price Amount => Price.Create(_orderItems.Sum(i => i.TotalPrice.Value));
    public Audit Audit { get; set; }

    public int Id { get; }
    public SoftDelete SoftDelete { get; set; }

    public static Result<OrderEntity> Create(int userId, IReadOnlyCollection<OrderItemEntity> items)
    {
        var order = new OrderEntity(userId, items);

        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, order.UserId));
        return Result.Success(order);
    }

    public Result AddOrderItem(OrderItemEntity orderItem)
    {
        Guard.Against.Null(orderItem, nameof(orderItem));

        if (_orderItems.Any(c => c.Id == orderItem.Id))
            return Result.Conflict("OrderItem already exists");

        if (_orderItems.Any(c => c.ProductId == orderItem.ProductId))
            return Result.Conflict("An orderItem with this productId already exists");

        _orderItems.Add(orderItem);
        return Result.Success();
    }
}
