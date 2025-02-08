#region

using DemoShop.Api.Common.Base;
using DemoShop.Application.Features.User.Interfaces;

#endregion

namespace DemoShop.Api.Features.User.Models;

public sealed record UpdateUserPhoneRequest : Request, IUpdateUserPhoneRequest
{
    public string? Phone { get; init; }
}
