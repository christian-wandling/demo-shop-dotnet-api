namespace DemoShop.Application.Features.Order.DTOs;

public sealed record OrderItemResponse
{
    public required int ProductId { get; init; }
    public required string ProductName { get; init; }
    public required string ProductThumbnail { get; init; }
    public required int Quantity { get; init; }
    public required int UnitPrice { get; init; }
    public required int TotalPrice { get; init; }
}
