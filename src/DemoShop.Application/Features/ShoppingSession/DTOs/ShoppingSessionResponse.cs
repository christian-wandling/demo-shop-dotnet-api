namespace DemoShop.Application.Features.ShoppingSession.DTOs;

public sealed record ShoppingSessionResponse
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required IReadOnlyCollection<CartItemResponse> Items { get; init; }
}
