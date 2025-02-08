#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.DeleteShoppingSession;

public sealed class DeleteShoppingSessionCommandHandler(
    IShoppingSessionRepository repository,
    IDomainEventDispatcher eventDispatcher,
    ILogger<DeleteShoppingSessionCommandHandler> logger
)
    : IRequestHandler<DeleteShoppingSessionCommand, Result>
{
    public async Task<Result> Handle(DeleteShoppingSessionCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Session, nameof(request.Session));

        try
        {
            var deleted = await repository.DeleteSessionAsync(request.Session, cancellationToken);

            if (!deleted)
                return Result.Error("The shopping session was not deleted");

            await eventDispatcher.DispatchEventsAsync(request.Session, cancellationToken);

            return Result.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogOperationFailed("Delete shopping session", "Id", $"{request.Session.Id}", ex);
            return Result.Error(ex.Message);
        }
    }
}
