using Ardalis.Result;
using DemoShop.Application.Features.Order.DTOs;
using MediatR;

namespace DemoShop.Application.Features.Order.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(int Id) : IRequest<Result<OrderResponse?>>;
