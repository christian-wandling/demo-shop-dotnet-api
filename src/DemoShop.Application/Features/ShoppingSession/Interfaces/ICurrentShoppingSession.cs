#region

using Ardalis.Result;
using DemoShop.Domain.ShoppingSession.Entities;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Interfaces;

public interface ICurrentShoppingSessionAccessor
{
    Task<Result<ShoppingSessionEntity>> GetCurrent(CancellationToken cancellationToken);
}
