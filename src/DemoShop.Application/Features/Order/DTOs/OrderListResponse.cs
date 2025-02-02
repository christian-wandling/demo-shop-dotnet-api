namespace DemoShop.Application.Features.Order.DTOs;

public sealed record OrderListResponse
{
    public required IReadOnlyCollection<OrderResponse> Items { get; init; }
}
