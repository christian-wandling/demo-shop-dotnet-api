#region

using DemoShop.Api.Common.Base;
using DemoShop.Application.Features.Product.Interfaces;

#endregion

namespace DemoShop.Api.Features.Product.Models;

public sealed record GetProductByIdRequest : Request, IGetProductByIdRequest
{
    public required int Id { get; init; }
}
