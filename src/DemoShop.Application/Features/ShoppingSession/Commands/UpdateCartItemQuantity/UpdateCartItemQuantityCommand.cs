using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using MediatR;

namespace DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;

public sealed record UpdateCartItemQuantityCommand(int Id, IUpdateCartItemRequest UpdateCartItem) : IRequest<Result<UpdateCartItemQuantityResponse>>;
