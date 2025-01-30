using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;

public sealed class GetShoppingSessionByUserIdHandler(
    IMapper mapper,
    IShoppingSessionRepository repository,
    ILogger<GetShoppingSessionByUserIdHandler> logger)
    : IRequestHandler<GetShoppingSessionByUserIdQuery, Result<ShoppingSessionResponse>>
{
    public async Task<Result<ShoppingSessionResponse>> Handle(GetShoppingSessionByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await repository.GetSessionByUserIdAsync(request.UserId, cancellationToken)
            .ConfigureAwait(false);

        if (user is not null)
            return Result.Success(mapper.Map<ShoppingSessionResponse>(user));

        logger.LogOperationFailed("Get ShoppingSession By UserId", "UserId", $"{request.UserId}", null);
        return Result.Error("ShoppingSession not found");
    }
}
