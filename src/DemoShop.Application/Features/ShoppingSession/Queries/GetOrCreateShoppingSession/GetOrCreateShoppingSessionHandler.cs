using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;
using DemoShop.Application.Features.User.Interfaces;
using MediatR;

namespace DemoShop.Application.Features.ShoppingSession.Queries.GetOrCreateShoppingSession;

public sealed class GetOrCreateShoppingSessionHandler(
    ICurrentUserAccessor user,
    IMediator mediator)
    : IRequestHandler<GetOrCreateShoppingSessionQuery, Result<ShoppingSessionResponse>>
{
    public async Task<Result<ShoppingSessionResponse>> Handle(GetOrCreateShoppingSessionQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var userIdResult = await user.GetId(cancellationToken).ConfigureAwait(false);

        if (!userIdResult.IsSuccess)
        {
            return Result.Forbidden("Authorization Failed");
        }

        var sessionResult = await mediator
            .Send(new GetShoppingSessionByUserIdQuery(userIdResult.Value), cancellationToken)
            .ConfigureAwait(false);

        if (sessionResult.IsError())
        {
            sessionResult = await mediator
                .Send(
                    new CreateShoppingSessionCommand(userIdResult.Value),
                    cancellationToken
                ).ConfigureAwait(false);
        }

        return sessionResult;
    }
}
