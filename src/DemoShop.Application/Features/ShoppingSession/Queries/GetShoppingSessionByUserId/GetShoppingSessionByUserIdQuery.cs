using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;

public sealed record GetShoppingSessionByUserIdQuery(int UserId) : IRequest<Result<ShoppingSessionResponse?>>;
