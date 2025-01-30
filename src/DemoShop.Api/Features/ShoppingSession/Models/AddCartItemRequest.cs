using DemoShop.Application.Features.ShoppingSession.Interfaces;

namespace DemoShop.Api.Features.ShoppingSession.Models;

public sealed record AddCartItemRequest : IAddCartItemRequest
{
    public int ProductId { get;  set; }
}
