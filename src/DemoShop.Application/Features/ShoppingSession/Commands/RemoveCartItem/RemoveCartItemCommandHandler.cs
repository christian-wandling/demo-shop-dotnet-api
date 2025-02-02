using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.Common.Extensions;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Queries.GetOrCreateShoppingSession;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;

public sealed class RemoveCartItemCommandHandler(
    ICurrentShoppingSessionAccessor currentSession,
    IShoppingSessionRepository repository,
    IDomainEventDispatcher eventDispatcher
)
    : IRequestHandler<RemoveCartItemCommand, Result>
{
    public async Task<Result> Handle(RemoveCartItemCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var currentSessionResult = await currentSession.GetCurrent(cancellationToken).ConfigureAwait(false);

        Guard.Against.Null(currentSessionResult, nameof(currentSessionResult));

        currentSessionResult.Value.RemoveCartItem(request.Id);

        await repository.UpdateSessionAsync(currentSessionResult.Value, cancellationToken)
            .ConfigureAwait(false);

        await eventDispatcher.DispatchEventsAsync(currentSessionResult.Value, cancellationToken).ConfigureAwait(false);

        return Result.NoContent();
    }
}
