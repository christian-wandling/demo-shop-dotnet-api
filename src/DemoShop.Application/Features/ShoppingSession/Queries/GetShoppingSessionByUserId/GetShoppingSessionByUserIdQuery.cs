#region

using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;

public sealed record GetShoppingSessionByUserIdQuery(int UserId) : IRequest<Result<ShoppingSessionResponse>>;
