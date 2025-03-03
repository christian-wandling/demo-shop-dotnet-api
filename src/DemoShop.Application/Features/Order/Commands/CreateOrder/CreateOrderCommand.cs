#region

using Ardalis.Result;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.ShoppingSession.Entities;
using MediatR;

#endregion

namespace DemoShop.Application.Features.Order.Commands.ConvertShoppingSessionToOrder;

public sealed record CreateOrderCommand(ShoppingSessionEntity Session)
    : IRequest<Result<OrderEntity>>;
