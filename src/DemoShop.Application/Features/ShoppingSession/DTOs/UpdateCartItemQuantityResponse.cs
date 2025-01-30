namespace DemoShop.Application.Features.ShoppingSession.DTOs;

public sealed record UpdateCartItemQuantityResponse
{
    public required int Quantity { get; init; }
}
