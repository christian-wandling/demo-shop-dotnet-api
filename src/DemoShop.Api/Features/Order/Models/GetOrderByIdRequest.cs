#region

using DemoShop.Api.Common.Base;
using DemoShop.Application.Features.Order.Interfaces;

#endregion

namespace DemoShop.Api.Features.Order.Models;

public sealed record GetOrderByIdRequest : Request, IGetOrderByIdRequest
{
    public required int Id { get; init; }
}
