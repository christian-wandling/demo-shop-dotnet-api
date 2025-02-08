#region

using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Queries.GetOrCreateShoppingSession;

public record GetOrCreateShoppingSessionQuery : IRequest<Result<ShoppingSessionResponse>>;
