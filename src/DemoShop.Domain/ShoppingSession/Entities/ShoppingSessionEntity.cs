#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.ShoppingSession.Events;
using DemoShop.Domain.User.Entities;

#endregion

namespace DemoShop.Domain.ShoppingSession.Entities;

public sealed class ShoppingSessionEntity : IEntity, IAuditable, IAggregateRoot
{
    private readonly List<CartItemEntity> _cartItems = [];

    private ShoppingSessionEntity()
    {
        Audit = Audit.Create();
    }

    private ShoppingSessionEntity(int userId)
    {
        UserId = Guard.Against.NegativeOrZero(userId, nameof(userId));
        Audit = Audit.Create();
    }

    public int UserId { get; }
    public UserEntity? User { get; init; }
    public IReadOnlyCollection<CartItemEntity> CartItems => _cartItems.AsReadOnly();
    public Audit Audit { get; }

    public int Id { get; }

    public static Result<ShoppingSessionEntity> Create(int userId)
    {
        var session = new ShoppingSessionEntity(userId);
        session.AddDomainEvent(new ShoppingSessionCreated(session));

        return Result.Success(session);
    }

    public Result<CartItemEntity> AddCartItem(int productId)
    {
        var cartItemResult = CartItemEntity.Create(Id, productId);

        if (!cartItemResult.IsSuccess)
            return cartItemResult;

        if (_cartItems.Any(c => c.ProductId == cartItemResult.Value.ProductId))
            return Result.Conflict("Product already in cart");

        _cartItems.Add(cartItemResult.Value);
        Audit.UpdateModified();
        this.AddDomainEvent(new CartItemAdded(cartItemResult.Value.Id, UserId));

        return cartItemResult;
    }

    public Result<CartItemEntity> UpdateCartItem(int cartItemId, int quantity)
    {
        var cartItemToUpdate = _cartItems.FirstOrDefault(x => x.Id == cartItemId);

        if (cartItemToUpdate is null) return Result.NotFound("CartItem not found");

        var oldQuantity = cartItemToUpdate.Quantity;
        cartItemToUpdate.UpdateQuantity(quantity);
        Audit.UpdateModified();
        this.AddDomainEvent(new CartItemQuantityChanged(cartItemToUpdate, oldQuantity.Value, UserId));

        return Result.Success(cartItemToUpdate);
    }

    public Result RemoveCartItem(int cartItemId)
    {
        var cartItemToRemove = _cartItems.FirstOrDefault(x => x.Id == cartItemId);

        if (cartItemToRemove is null) return Result.NotFound("CartItem not found");

        _cartItems.Remove(cartItemToRemove);

        this.AddDomainEvent(new CartItemRemoved(cartItemToRemove.Id, UserId));

        return Result.Success();
    }

    public Result<OrderEntity> ConvertToOrder()
    {
        if (_cartItems.Count is 0) return Result.Error("No items in shopping session");

        var orderItems = _cartItems.ConvertAll(cartItem => cartItem.ConvertToOrderItem().Value);
        if (orderItems.Count == 0)
            return Result.Error("No items in shopping session");

        var orderResult = OrderEntity.Create(UserId, orderItems);
        if (!orderResult.IsSuccess)
            return orderResult.Map();

        this.AddDomainEvent(new ShoppingSessionConverted(this, orderResult.Value.Id));

        return orderResult;
    }
}
