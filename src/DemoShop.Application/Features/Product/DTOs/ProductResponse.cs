namespace DemoShop.Application.Features.Product.DTOs;

public sealed record ProductResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required IReadOnlyCollection<string> Categories { get; init; }
    public required IReadOnlyCollection<ImageResponse> Images { get; init; }
}
