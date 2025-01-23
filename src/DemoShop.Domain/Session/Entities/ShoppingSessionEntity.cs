using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Session.Entities;

public class ShoppingSessionEntity : IEntity, IAuditable, IAggregateRoot
{
    public required int UserId { get; init; }

    public ICollection<CartItemEntity> CartItems { get; } = new List<CartItemEntity>();
    public User.Entities.UserEntity? User { get; init; }
    public int Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; }
    public required Audit Audit { get; set; }
}
