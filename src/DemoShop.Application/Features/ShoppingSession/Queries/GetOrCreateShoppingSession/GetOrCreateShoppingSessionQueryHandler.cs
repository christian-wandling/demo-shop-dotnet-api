#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Queries.GetOrCreateShoppingSession;

public sealed class GetOrCreateShoppingSessionQueryHandler(
    ICurrentUserAccessor user,
    IMediator mediator,
    ILogger<GetOrCreateShoppingSessionQueryHandler> logger
)
    : IRequestHandler<GetOrCreateShoppingSessionQuery, Result<ShoppingSessionResponse>>
{
    public async Task<Result<ShoppingSessionResponse>> Handle(GetOrCreateShoppingSessionQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var userIdResult = await user.GetId(cancellationToken);

        if (!userIdResult.IsSuccess) return Result.Forbidden("Authorization Failed");

        try
        {
            var result = await mediator
                .Send(new GetShoppingSessionByUserIdQuery(userIdResult.Value), cancellationToken);

            if (!result.IsSuccess)
                result = await mediator.Send(new CreateShoppingSessionCommand(userIdResult.Value), cancellationToken);

            return result;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            logger.LogOperationFailed("Get Session By UserId", "UserId", $"{userIdResult.Value}", ex);
            return Result.Error(ex.Message);
        }
    }
}
