#region

using DemoShop.Api.Common.Base;
using DemoShop.Application.Features.ShoppingSession.Interfaces;

#endregion

namespace DemoShop.Api.Features.ShoppingSession.Models;

public sealed record UpdateCartItemQuantityRequest : Request, IUpdateCartItemRequest
{
    public int Quantity { get; init; }
}
