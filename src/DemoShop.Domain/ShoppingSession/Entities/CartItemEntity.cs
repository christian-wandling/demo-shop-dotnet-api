using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.ShoppingSession.Entities;

public class CartItemEntity : IEntity, IAuditable, IAggregateRoot
{
    public required int ShoppingSessionId { get; init; }
    public required int ProductId { get; init; }
    public required int Quantity { get; set; }

    public Product.Entities.ProductEntity? Product { get; init; }
    public ShoppingSessionEntity? ShoppingSession { get; init; }
    public int Id { get; }
    public required Audit Audit { get; set; }
}
