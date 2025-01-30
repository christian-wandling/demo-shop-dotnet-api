using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;
using MediatR;

namespace DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;

public sealed record RemoveCartItemCommand(int Id) : IRequest<Result>;
