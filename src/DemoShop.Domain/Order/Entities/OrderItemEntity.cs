#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.ValueObjects;

#endregion

namespace DemoShop.Domain.Order.Entities;

public sealed class OrderItemEntity : IEntity, IAuditable, ISoftDeletable
{
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

    public int OrderId { get; private set; }
    public OrderEntity Order { get; private set; } = null!;
    public int ProductId { get; private set; }
    public OrderProduct Product { get; private init; }
    public Quantity Quantity { get; }
    public Price Price { get; }

    public Price TotalPrice => Price.Multiply(Quantity.Value);
    public Audit Audit { get; }

    public int Id { get; }
    public SoftDelete SoftDelete { get; }

    public static Result<OrderItemEntity> Create(
        int productId,
        OrderProduct product,
        Quantity quantity,
        Price price
    )
    {
        var orderItem = new OrderItemEntity(productId, product, quantity, price);

        return Result.Success(orderItem);
    }
}
