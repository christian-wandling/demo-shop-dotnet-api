using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.ShoppingSession.Entities;
using MediatR;

namespace DemoShop.Application.Features.ShoppingSession.Commands.DeleteShoppingSession;

public sealed record DeleteShoppingSessionCommand(ShoppingSessionEntity Session) : IRequest<Result>;
