#region

using DemoShop.Api.Common.Base;
using DemoShop.Application.Features.ShoppingSession.Interfaces;

#endregion

namespace DemoShop.Api.Features.ShoppingSession.Models;

public sealed record AddCartItemRequest : Request, IAddCartItemRequest
{
    public int ProductId { get; init; }
}
