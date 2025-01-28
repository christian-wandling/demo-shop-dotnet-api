namespace DemoShop.Domain.Product.DTOs;

public sealed record CreateImageDto()
{
    public required string Name { get; init; }
    public required Uri Uri { get; init; }
}
