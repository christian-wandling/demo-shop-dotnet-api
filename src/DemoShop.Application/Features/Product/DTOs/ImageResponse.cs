namespace DemoShop.Application.Features.Product.DTOs;

public sealed record ImageResponse
{
    public required string Name { get; init; }
    public required Uri Uri { get; init; }
}
