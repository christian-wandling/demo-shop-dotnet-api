namespace DemoShop.Application.Features.User.DTOs;

public sealed record UserPhoneResponse
{
    public required string? Phone { get; init; }
}
