using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Exceptions;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.Events;
using DemoShop.Domain.User.Events;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.ShoppingSession.Entities;

public class CartItemEntity : IEntity, IAuditable
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

    public int Id { get; }
    public int ShoppingSessionId { get; private init; }
    public int ProductId { get; private init; }
    public Quantity Quantity { get; private set; }
    public ProductEntity? Product { get; init; }
    public ShoppingSessionEntity? ShoppingSession { get; init; }
    public Audit Audit { get; }

    public static CartItemEntity Create(int shoppingSessionId, int productId)
    {
        Guard.Against.NegativeOrZero(shoppingSessionId, nameof(shoppingSessionId));
        Guard.Against.NegativeOrZero(productId, nameof(productId));

        return new CartItemEntity(shoppingSessionId, productId);
    }

    public void UpdateQuantity(int quantity) => Quantity = Quantity.Create(quantity);
}
