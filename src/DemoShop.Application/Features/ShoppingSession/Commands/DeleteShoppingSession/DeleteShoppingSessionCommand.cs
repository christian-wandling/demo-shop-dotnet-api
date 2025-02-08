#region

using Ardalis.Result;
using DemoShop.Domain.ShoppingSession.Entities;
using MediatR;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.DeleteShoppingSession;

public sealed record DeleteShoppingSessionCommand(ShoppingSessionEntity Session) : IRequest<Result>;
