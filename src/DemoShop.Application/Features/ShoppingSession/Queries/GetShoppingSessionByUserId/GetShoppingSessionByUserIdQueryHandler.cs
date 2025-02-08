#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;

public sealed class GetShoppingSessionByUserIdQueryHandler(
    IMapper mapper,
    IShoppingSessionRepository repository,
    ILogger<GetShoppingSessionByUserIdQueryHandler> logger)
    : IRequestHandler<GetShoppingSessionByUserIdQuery, Result<ShoppingSessionResponse>>
{
    public async Task<Result<ShoppingSessionResponse>> Handle(GetShoppingSessionByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        try
        {
            var user = await repository.GetSessionByUserIdAsync(request.UserId, cancellationToken);

            if (user is not null)
                return Result.Success(mapper.Map<ShoppingSessionResponse>(user));

            logger.LogOperationFailed("Get ShoppingSession By UserId", "UserId", $"{request.UserId}", null);
            return Result.Error("ShoppingSession not found");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            logger.LogOperationFailed("Get ShoppingSession By UserId", "UserId", $"{request.UserId}", ex);
            return Result.Error(ex.Message);
        }
    }
}
