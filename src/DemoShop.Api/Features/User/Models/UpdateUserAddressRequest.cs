#region

using DemoShop.Api.Common.Base;
using DemoShop.Application.Features.User.Interfaces;

#endregion

namespace DemoShop.Api.Features.User.Models;

public sealed record UpdateUserAddressRequest : Request, IUpdateUserAddressRequest
{
    public required string Street { get; set; }
    public required string Apartment { get; set; }
    public required string City { get; set; }
    public required string Zip { get; set; }
    public required string Country { get; set; }
    public string? Region { get; set; }
}
