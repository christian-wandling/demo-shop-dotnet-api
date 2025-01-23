namespace DemoShop.Domain.Session.Operations;

public class UpdateCartItemQuantity
{
    public required int Id { get; init; }
    public required int Quantity { get; init; }
}
