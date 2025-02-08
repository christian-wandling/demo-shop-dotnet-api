#region

using Ardalis.Result;
using DemoShop.Application.Features.Order.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.Order.Commands.CreateOrder;

public sealed record CreateOrderCommand : IRequest<Result<OrderResponse>>;
