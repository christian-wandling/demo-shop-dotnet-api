#region

using DemoShop.Api.Common.Base;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DemoShop.Api.Features.ShoppingSession.Models;

public sealed record UpdateCartItemQuantityRequestWrapper : Request
{
    [FromRoute] public required int Id { get; init; }
    [FromBody] public required UpdateCartItemQuantityRequest UpdateCartItemQuantityRequest { get; init; }
}
