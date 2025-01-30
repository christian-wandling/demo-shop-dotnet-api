using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Exceptions;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.ShoppingSession.Events;
using DemoShop.Domain.User.Entities;

namespace DemoShop.Domain.ShoppingSession.Entities;

public class ShoppingSessionEntity : IEntity, IAuditable, IAggregateRoot
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

    public int Id { get; }
    public int UserId { get; private init; }
    public UserEntity? User { get; init; }
    public IReadOnlyCollection<CartItemEntity> CartItems => _cartItems.AsReadOnly();
    public Audit Audit { get; }

    public static ShoppingSessionEntity Create(int userId)
    {
        var session = new ShoppingSessionEntity(userId);
        session.AddDomainEvent(new ShoppingSessionCreated(session));
        return session;
    }

    public Result<CartItemEntity> AddCartItem(int productId)
    {
        var createdCartItem = CartItemEntity.Create(Id, productId);

        Guard.Against.Null(createdCartItem, nameof(createdCartItem));

        if (_cartItems.Any(c => c.ProductId == createdCartItem.ProductId))
        {
            throw new AlreadyExistsException("CartItem already added");
        }

        _cartItems.Add(createdCartItem);
        Audit.UpdateModified();
        this.AddDomainEvent(new CartItemAdded(createdCartItem));

        return Result.Success(createdCartItem);
    }

    public Result<CartItemEntity> UpdateCartItem(int cartItemId, int quantity)
    {
        var cartItemToUpdate = _cartItems.FirstOrDefault(x => x.Id == cartItemId);

        Guard.Against.Null(cartItemToUpdate, nameof(cartItemToUpdate));

        var oldQuantity = cartItemToUpdate.Quantity;
        cartItemToUpdate.UpdateQuantity(quantity);
        Audit.UpdateModified();
        this.AddDomainEvent(new CartItemQuantityChanged(cartItemToUpdate, oldQuantity.Value));

        return Result.Success(cartItemToUpdate);
    }

    public void RemoveCartItem(int cartItemId)
    {
        var cartItemToRemove = _cartItems.FirstOrDefault(x => x.Id == cartItemId);

        Guard.Against.Null(cartItemToRemove, nameof(cartItemToRemove));

        _cartItems.Remove(cartItemToRemove);

        this.AddDomainEvent(new CartItemRemoved(this, cartItemToRemove));
    }
}
