using DemoShop.Domain.Session.Entities;

namespace DemoShop.Domain.Order.Operations;

public class CreateOrder
{
    public required int ShoppingSessionId { get; set; }
    public required int UserId { get; set; }
    public ICollection<CartItemEntity> CartItems { get; } = new List<CartItemEntity>();
}
