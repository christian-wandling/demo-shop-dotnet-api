using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Common.Extensions;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;

public sealed class CreateShoppingSessionCommandHandler(
    IMapper mapper,
    IShoppingSessionRepository repository,
    ILogger<CreateShoppingSessionCommandHandler> logger,
    IDomainEventDispatcher eventDispatcher,
    IValidator<CreateShoppingSessionCommand> validator
)
    : IRequestHandler<CreateShoppingSessionCommand, Result<ShoppingSessionResponse>>
{
    public async Task<Result<ShoppingSessionResponse>> Handle(CreateShoppingSessionCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NegativeOrZero(request.UserId, nameof(request.UserId));

        var validationResult = await validator.ValidateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            logger.LogValidationFailed("Create ShoppingSession", string.Join(", ", errors));

            return Result.Invalid(validationResult.Errors.ToValidationErrors());
        }

        var createdSession = ShoppingSessionEntity.Create(request.UserId);
        var session = await repository.CreateSessionAsync(createdSession, cancellationToken)
            .ConfigureAwait(false);

        if (session is null)
        {
            logger.LogOperationFailed("Create ShoppingSession", "UserId", $"{request.UserId}", null);
            return Result.Error("Failed to create ShoppingSession");
        }

        await eventDispatcher.DispatchEventsAsync(session, cancellationToken).ConfigureAwait(false);
        return Result.Success(mapper.Map<ShoppingSessionResponse>(session));
    }
}
