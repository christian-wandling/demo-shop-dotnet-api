#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.ValueObjects;
using DemoShop.Domain.Product.Entities;

#endregion

namespace DemoShop.Domain.ShoppingSession.Entities;

public sealed class CartItemEntity : IEntity, IAuditable
{
    private CartItemEntity()
    {
        Quantity = Quantity.Create(1);
        Audit = Audit.Create();
    }

    private CartItemEntity(int shoppingSessionId, int productId)
    {
        ShoppingSessionId = Guard.Against.NegativeOrZero(shoppingSessionId, nameof(shoppingSessionId));
        ProductId = Guard.Against.NegativeOrZero(productId, nameof(productId));
        Quantity = Quantity.Create(1);
        Audit = Audit.Create();
    }

    public int ShoppingSessionId { get; private init; }
    public ShoppingSessionEntity? ShoppingSession { get; init; }
    public int ProductId { get; init; }
    public ProductEntity? Product { get; init; }
    public Quantity Quantity { get; private set; }

    public decimal TotalPrice => Product?.Price.Value * Quantity.Value ?? 0;
    public Audit Audit { get; }

    public int Id { get; }

    public static Result<CartItemEntity> Create(int shoppingSessionId, int productId)
    {
        var cartItem = new CartItemEntity(shoppingSessionId, productId);

        return Result.Success(cartItem);
    }

    public Result<OrderItemEntity> ConvertToOrderItem()
    {
        if (Product is null)
            return Result.CriticalError("No product");

        return OrderItemEntity.Create(
            ProductId,
            OrderProduct.Create(Product.Name, Product.Thumbnail ?? string.Empty),
            Quantity,
            Product!.Price
        );
    }

    public void UpdateQuantity(int quantity) => Quantity = Quantity.Create(quantity);
}
