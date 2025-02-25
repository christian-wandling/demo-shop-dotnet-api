#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;

public sealed class RemoveCartItemCommandHandler(
    ICurrentShoppingSessionAccessor currentSession,
    IShoppingSessionRepository repository,
    IDomainEventDispatcher eventDispatcher,
    ILogger<RemoveCartItemCommandHandler> logger,
    IValidator<RemoveCartItemCommand> validator,
    IValidationService validationService
)
    : IRequestHandler<RemoveCartItemCommand, Result>
{
    public async Task<Result> Handle(RemoveCartItemCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NegativeOrZero(request.Id, nameof(request.Id));

        var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);

        if (!validationResult.IsSuccess)
            return validationResult.Map();

        var sessionResult = await currentSession.GetCurrent(cancellationToken);

        if (!sessionResult.IsSuccess)
            return sessionResult.Map();

        try
        {
            var unsavedResult = sessionResult.Value.RemoveCartItem(request.Id);

            if (!unsavedResult.IsSuccess)
                return unsavedResult;

            var savedResult = await SaveChanges(sessionResult, cancellationToken);

            return savedResult.IsSuccess
                ? Result.NoContent()
                : savedResult.Map();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogOperationFailed("Remove cart item", "Id", $"{request.Id}", ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<Result<ShoppingSessionEntity>> SaveChanges(ShoppingSessionEntity unsavedSession,
        CancellationToken cancellationToken)
    {
        var savedSession = await repository.UpdateSessionAsync(unsavedSession, cancellationToken);

        await eventDispatcher.DispatchEventsAsync(unsavedSession, cancellationToken);
        return Result.Success(savedSession);
    }
}
