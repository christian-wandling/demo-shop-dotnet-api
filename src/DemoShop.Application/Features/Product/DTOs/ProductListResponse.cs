namespace DemoShop.Application.Features.Product.DTOs;

public sealed record ProductListResponse
{
    public required IReadOnlyCollection<ProductResponse> Products { get; init; }
}
