namespace DemoShop.Application.Features.Users.DTOs;

public class UserResponse
{
    public int Id { get; init; }
    public required string Email { get; init; }
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
    public required string? Phone { get; init; }
    public required AddressResponse? Address { get; init; }
}
