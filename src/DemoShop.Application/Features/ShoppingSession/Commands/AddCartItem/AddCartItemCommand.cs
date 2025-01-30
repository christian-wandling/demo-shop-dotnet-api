using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;
using MediatR;

namespace DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;

public sealed record AddCartItemCommand(IAddCartItemRequest AddCartItem) : IRequest<Result<CartItemResponse>>;
