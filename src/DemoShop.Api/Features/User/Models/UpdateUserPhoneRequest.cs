namespace DemoShop.Api.Features.User.Models;

public sealed record UpdateUserPhoneRequest
{
    public string? PhoneNumber { get; set; }
}
