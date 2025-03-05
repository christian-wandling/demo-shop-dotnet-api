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
using Serilog;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;

public sealed class CreateShoppingSessionCommandHandler(
    IMapper mapper,
    IShoppingSessionRepository repository,
    ILogger logger,
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

        try
        {
            LogCommandStarted(logger, request.UserId);

            var validationResult = await validationService.ValidateAsync(request, validator, cancellationToken);
            if (!validationResult.IsSuccess)
            {
                LogCommandError(logger, request.UserId);
                return validationResult.Map();
            }

            var unsavedResult = ShoppingSessionEntity.Create(request.UserId);

            if (!unsavedResult.IsSuccess)
            {
                LogCommandError(logger, request.UserId);
                return Result.Error("Failed to create shopping session");
            }

            var savedResult = await SaveChanges(unsavedResult, cancellationToken);
            if (!savedResult.IsSuccess)
            {
                LogCommandError(logger, request.UserId);
                return savedResult.Map();
            }

            LogCommandSuccess(logger, savedResult.Value.Id, savedResult.Value.UserId);
            return Result.Success(mapper.Map<ShoppingSessionResponse>(savedResult.Value));
        }
        catch (InvalidOperationException ex)
        {
            LogInvalidOperationException(logger, ex.Message, ex);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            LogDatabaseException(logger, ex.Message, ex);
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

    private static void LogCommandStarted(ILogger logger, int userId) => logger
        .ForContext("EventId", LoggerEventId.CreateShoppingSessionCommandStarted)
        .Debug("Starting to create shopping session with UserId {UserId}", userId);

    private static void LogCommandSuccess(ILogger logger, int sessionId, int userId) => logger
        .ForContext("EventId", LoggerEventId.CreateShoppingSessionCommandSuccess)
        .Information("Successfully created shopping session with Id {SessionId} for UserId {UserId}",
            sessionId, userId);

    private static void LogCommandError(ILogger logger, int userId) => logger
        .ForContext("EventId", LoggerEventId.CreateShoppingSessionCommandError)
        .Error("Error creating shopping session with UserId {UserId}", userId);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.CreateShoppingSessionDatabaseException)
        .Error(ex, "Database error occurred while creating shopping session. Error: {ErrorMessage}", errorMessage);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.CreateShoppingSessionDomainException)
        .Error(ex, "Invalid operation while creating shopping session. Error: {ErrorMessage}", errorMessage);
}
