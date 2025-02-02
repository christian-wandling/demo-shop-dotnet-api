using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.ShoppingSession.Queries.GetOrCreateShoppingSession;

public record GetOrCreateShoppingSessionQuery() : IRequest<Result<ShoppingSessionResponse?>>;
