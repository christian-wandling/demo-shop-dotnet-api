using DemoShop.Application.Features.ShoppingSession.Interfaces;

namespace DemoShop.Api.Features.ShoppingSession.Models;

public sealed record UpdateCartItemRequest : IUpdateCartItemRequest
{
    public int Quantity { get; set; }
}
