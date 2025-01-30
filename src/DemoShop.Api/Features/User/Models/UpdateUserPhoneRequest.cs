using DemoShop.Application.Features.User.Interfaces;

namespace DemoShop.Api.Features.User.Models;

public sealed record UpdateUserPhoneRequest : IUpdateUserPhoneRequest
{
    public string? Phone { get; set; }
}
