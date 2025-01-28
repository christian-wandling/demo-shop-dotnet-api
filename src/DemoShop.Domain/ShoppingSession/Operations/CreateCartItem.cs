namespace DemoShop.Domain.ShoppingSession.Operations;

public class CreateCartItem
{
    public required int ProductId { get; init; }
    public required int Quantity { get; init; } = 1;
}
