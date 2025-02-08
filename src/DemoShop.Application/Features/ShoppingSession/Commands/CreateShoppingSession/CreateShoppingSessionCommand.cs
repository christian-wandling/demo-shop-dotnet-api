#region

using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;

public sealed record CreateShoppingSessionCommand(int UserId) : IRequest<Result<ShoppingSessionResponse>>;
