using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.ShoppingSession.Interfaces;
using MediatR;

namespace DemoShop.Application.Features.ShoppingSession.Commands.DeleteShoppingSession;

public sealed class DeleteShoppingSessionCommandHandler(
    IShoppingSessionRepository repository,
    IDomainEventDispatcher eventDispatcher
)
    : IRequestHandler<DeleteShoppingSessionCommand, Result>
{
    public async Task<Result> Handle(DeleteShoppingSessionCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Session, nameof(request.Session));

        await repository.DeleteSessionAsync(request.Session, cancellationToken)
            .ConfigureAwait(false);

        await eventDispatcher.DispatchEventsAsync(request.Session, cancellationToken).ConfigureAwait(false);
        return Result.NoContent();
    }
}
