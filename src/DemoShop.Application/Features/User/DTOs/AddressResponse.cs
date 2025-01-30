namespace DemoShop.Application.Features.User.DTOs;

public sealed class AddressResponse
{
    public required string Street { get; init; }
    public required string Apartment { get; init; }
    public required string City { get; init; }
    public required string Zip { get; init; }
    public string? Region { get; init; }
    public required string Country { get; init; }
}
