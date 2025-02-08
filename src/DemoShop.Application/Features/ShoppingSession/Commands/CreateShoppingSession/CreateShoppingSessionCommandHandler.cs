#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;

public sealed class CreateShoppingSessionCommandHandler(
    IMapper mapper,
    IShoppingSessionRepository repository,
    ILogger<CreateShoppingSessionCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<CreateShoppingSessionCommand> validator,
    IValidationService validationService
)
    : IRequestHandler<CreateShoppingSessionCommand, Result<ShoppingSessionResponse>>
{
    public async Task<Result<ShoppingSessionResponse>> Handle(CreateShoppingSessionCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NegativeOrZero(request.UserId, nameof(request.UserId));

        var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);

        if (!validationResult.IsSuccess)
            return validationResult.Map();

        try
        {
            var unsavedResult = ShoppingSessionEntity.Create(request.UserId);

            if (!unsavedResult.IsSuccess)
                return Result.Error("Failed to create shopping session");

            var savedResult = await SaveChanges(unsavedResult, cancellationToken);

            return savedResult.IsSuccess
                ? Result.Success(mapper.Map<ShoppingSessionResponse>(savedResult.Value))
                : savedResult.Map();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogOperationFailed("Create shopping session", "UserId", $"{request.UserId}", ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<Result<ShoppingSessionEntity>> SaveChanges(ShoppingSessionEntity unsavedSession,
        CancellationToken cancellationToken)
    {
        var savedSession = await repository.CreateSessionAsync(unsavedSession, cancellationToken);

        if (savedSession is null)
            return Result.Error("Failed to create shopping session");

        await eventDispatcher.DispatchEventsAsync(unsavedSession, cancellationToken);
        return Result.Success(savedSession);
    }
}
