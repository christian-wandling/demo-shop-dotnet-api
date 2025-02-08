#region

using DemoShop.Domain.Order.Enums;

#endregion

namespace DemoShop.Application.Features.Order.DTOs;

public sealed record OrderResponse
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required IReadOnlyCollection<OrderItemResponse> Items { get; init; }
    public required decimal Amount { get; init; }
    public required OrderStatus Status { get; init; }
    public required DateTime Created { get; init; }
}
