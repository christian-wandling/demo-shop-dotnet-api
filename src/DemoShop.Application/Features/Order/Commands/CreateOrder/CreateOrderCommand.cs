using Ardalis.Result;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Domain.Order.Entities;
using MediatR;

namespace DemoShop.Application.Features.Order.Commands.CreateOrder;

public sealed record CreateOrderCommand() : IRequest<Result<OrderResponse?>>;
