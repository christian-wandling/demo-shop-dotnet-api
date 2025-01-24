namespace DemoShop.Domain.User.DTOs;

public sealed record UpdatePhoneDto
{
    public required string Phone { get; init; }
}
