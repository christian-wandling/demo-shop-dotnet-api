using Ardalis.Result;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Product.DTOs;
using MediatR;

namespace DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;

public sealed record GetAllOrdersOfUserQuery() : IRequest<Result<OrderListResponse>>;
