namespace DemoShop.Domain.User.DTOs;

public sealed record UpdateAddressDto
{
    public int AddressId { get; init; }
    public required int UserId { get; init; }
    public required string Street { get; init; }
    public required string Apartment { get; init; }
    public required string City { get; init; }
    public required string Zip { get; init; }
    public required string Country { get; init; }
    public string? Region { get; init; }
}
