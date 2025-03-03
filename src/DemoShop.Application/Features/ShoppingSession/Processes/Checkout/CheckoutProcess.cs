#region

using Ardalis.Result;
using DemoShop.Application.Features.Order.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Processes.Checkout;

public sealed record CheckoutProcess : IRequest<Result<OrderResponse>>;
