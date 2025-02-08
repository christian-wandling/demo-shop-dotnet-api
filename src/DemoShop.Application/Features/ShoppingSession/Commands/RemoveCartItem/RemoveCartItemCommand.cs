#region

using Ardalis.Result;
using MediatR;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;

public sealed record RemoveCartItemCommand(int Id) : IRequest<Result>;
