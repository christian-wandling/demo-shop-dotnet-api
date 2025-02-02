using Ardalis.Result;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.ShoppingSession.Entities;
using MediatR;

namespace DemoShop.Application.Features.Order.Commands.ConvertShoppingSessionToOrder;

public sealed record ConvertShoppingSessionToOrderCommand(ShoppingSessionEntity Session)
    : IRequest<Result<OrderEntity?>>;
