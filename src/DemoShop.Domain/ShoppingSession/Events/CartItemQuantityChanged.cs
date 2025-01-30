using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;

namespace DemoShop.Domain.ShoppingSession.Events;

public sealed record CartItemQuantityChanged(CartItemEntity CartItem, int OldQuantity) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
