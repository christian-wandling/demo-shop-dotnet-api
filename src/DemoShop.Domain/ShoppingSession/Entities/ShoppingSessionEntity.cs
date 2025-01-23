using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.ShoppingSession.Entities;

public class ShoppingSessionEntity : IEntity, IAuditable, IAggregateRoot
{
    public required int UserId { get; init; }

    public ICollection<CartItemEntity> CartItems { get; } = new List<CartItemEntity>();
    public UserEntity? User { get; init; }
    public int Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; }
    public required Audit Audit { get; set; }
}
