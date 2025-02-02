using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Exceptions;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.Events;
using DemoShop.Domain.Order.ValueObjects;

namespace DemoShop.Domain.Order.Entities;

public sealed class OrderItemEntity : IEntity, IAuditable, ISoftDeletable
{
    public int OrderId { get; private set; }

    private OrderItemEntity()
    {
        Product = OrderProduct.Empty;
        Quantity = Quantity.Create(1);
        Price = Price.Empty;
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    private OrderItemEntity(
        int productId,
        OrderProduct product,
        Quantity quantity,
        Price price
    )
    {
        ProductId = Guard.Against.NegativeOrZero(productId, nameof(productId));
        Product = Guard.Against.Null(product, nameof(product));
        Quantity = Guard.Against.Null(quantity, nameof(quantity));
        Price = Guard.Against.Null(price, nameof(price));
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    public int Id { get; }
    public OrderEntity Order { get; private set; } = null!;
    public int ProductId { get; private set; }
    public OrderProduct Product { get; private init; }
    public Quantity Quantity { get; private init; }
    public Price Price { get; private init; }
    public Audit Audit { get; }
    public SoftDelete SoftDelete { get; }

    public static OrderItemEntity Create(
        int productId,
        OrderProduct product,
        Quantity quantity,
        Price price
    ) =>
        new(productId, product, quantity, price);

    public Result Delete()
    {
        if (SoftDelete.Deleted)
            throw new AlreadyMarkedAsDeletedException($"OrderItem {Id} has already been marked as deleted");

        SoftDelete.MarkAsDeleted();
        Audit.UpdateModified();
        this.AddDomainEvent(new OrderItemDeletedDomainEvent(Id));

        return Result.Success();
    }

    public Result Restore()
    {
        if (SoftDelete.Deleted) throw new NotMarkedAsDeletedException($"OrderItem {Id} has not been marked as deleted");

        SoftDelete.MarkAsDeleted();
        Audit.UpdateModified();
        this.AddDomainEvent(new OrderItemRestoredDomainEvent(Id));

        return Result.Success();
    }

    public int TotalPrice => Price.Multiply(Quantity.Value).ToInt();
}
