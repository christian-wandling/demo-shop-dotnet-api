#region

using Ardalis.Result;
using DemoShop.Application.Features.Order.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;

public sealed record GetAllOrdersOfUserQuery : IRequest<Result<OrderListResponse>>;
