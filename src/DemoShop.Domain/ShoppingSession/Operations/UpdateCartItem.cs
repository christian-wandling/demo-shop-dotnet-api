namespace DemoShop.Domain.ShoppingSession.Operations;

public class UpdateCartItemQuantity
{
    public required int Id { get; init; }
    public required int Quantity { get; init; }
}
