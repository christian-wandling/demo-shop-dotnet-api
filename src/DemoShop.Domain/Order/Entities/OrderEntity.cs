using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.Enums;
using DemoShop.Domain.Order.Events;
using DemoShop.Domain.User.Entities;

namespace DemoShop.Domain.Order.Entities;

public class OrderEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
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

    public int Id { get; }
    public OrderStatus Status { get; private set; }
    public int UserId { get; init; }
    public UserEntity? User { get; init; }
    public Audit Audit { get; set; }
    public SoftDelete SoftDelete { get; set; }

    public IReadOnlyCollection<OrderItemEntity> OrderItems => _orderItems;

    public static Result<OrderEntity> Create(int userId, IReadOnlyCollection<OrderItemEntity> items)
    {
        var order = new OrderEntity(userId, items);

        order.AddDomainEvent(new OrderCreatedDomainEvent(order));
        return Result.Success(order);
    }

    public Result Delete()
    {
        if (SoftDelete.Deleted)
            return Result.Error("Order is already deleted");

        SoftDelete.MarkAsDeleted();
        this.AddDomainEvent(new OrderDeletedDomainEvent(Id));

        return Result.Success();
    }

    public Result Restore()
    {
        if (!SoftDelete.Deleted)
            return Result.Error("Order is not deleted");

        SoftDelete.MarkAsDeleted();
        this.AddDomainEvent(new OrderRestoredDomainEvent(Id));

        return Result.Success();
    }

    public Result AddOrderItem(OrderItemEntity orderItem)
    {
        Guard.Against.Null(orderItem, nameof(orderItem));

        if (_orderItems.Any(c => c.Id == orderItem.Id)) return Result.Error("OrderItem already exists");

        _orderItems.Add(orderItem);
        return Result.Success();
    }

    public int Amount => _orderItems.Aggregate(0, (acc, curr) => acc + curr.TotalPrice);
}
