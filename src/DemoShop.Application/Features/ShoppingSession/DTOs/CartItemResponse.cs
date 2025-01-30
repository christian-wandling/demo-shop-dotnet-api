namespace DemoShop.Application.Features.ShoppingSession.DTOs;

public sealed record CartItemResponse
{
    public required int Id { get; init; }
    public required int ProductId { get; init; }
    public required string ProductName { get; init; }
    public required string ProductThumbnail { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public required decimal TotalPrice { get; init; }
}
