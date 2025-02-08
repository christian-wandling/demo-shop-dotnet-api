#region

using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using MediatR;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;

public sealed record AddCartItemCommand(IAddCartItemRequest AddCartItem) : IRequest<Result<CartItemResponse>>;
