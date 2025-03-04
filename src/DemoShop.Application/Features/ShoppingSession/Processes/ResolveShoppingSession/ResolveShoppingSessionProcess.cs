#region

using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using MediatR;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Processes.ResolveShoppingSession;

public record ResolveShoppingSessionProcess : IRequest<Result<ShoppingSessionResponse>>;
