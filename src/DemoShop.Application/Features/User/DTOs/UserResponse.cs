namespace DemoShop.Application.Features.User.DTOs;

public sealed record UserResponse
{
    public required int Id { get; init; }
    public required string Email { get; init; }
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
    public required string? Phone { get; init; }
    public AddressResponse? Address { get; init; }
}
