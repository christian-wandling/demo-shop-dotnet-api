#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrderRepository repository,
    IDomainEventDispatcher eventDispatcher,
    ILogger logger
)
    : IRequestHandler<CreateOrderCommand, Result<OrderEntity>>
{
    public async Task<Result<OrderEntity>> Handle(CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Session, nameof(request.Session));

        try
        {
            LogCommandStarted(logger, request.Session.Id);

            var unsavedResult = request.Session.ConvertToOrder();
            if (!unsavedResult.IsSuccess)
            {
                LogCommandError(logger, request.Session.Id);
                return unsavedResult.Map();
            }

            var savedResult = await SaveChanges(unsavedResult.Value, cancellationToken);
            if (!savedResult.IsSuccess)
            {
                LogCommandError(logger, request.Session.Id);
                return savedResult.Map();
            }

            LogCommandSuccess(logger, request.Session.Id, savedResult.Value.Id);
            return savedResult;
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

    private async Task<Result<OrderEntity>> SaveChanges(OrderEntity unsavedOrder, CancellationToken cancellationToken)
    {
        var savedOrder = await repository.CreateOrderAsync(unsavedOrder, cancellationToken);

        if (savedOrder is null)
            return Result.Error("Failed to create order");

        await eventDispatcher.DispatchEventsAsync(unsavedOrder, cancellationToken);

        return Result.Success(savedOrder);
    }

    private static void LogCommandStarted(ILogger logger, int sessionId) => logger
        .ForContext("EventId", LoggerEventId.CreateOrderCommandStarted)
        .Debug("Starting to create order from shopping session with ID {SessionId}", sessionId);

    private static void LogCommandSuccess(ILogger logger, int orderId, int sessionId) => logger
        .ForContext("EventId", LoggerEventId.CreateOrderCommandSuccess)
        .Information("Successfully created order with Id {OrderId} from shopping session {SessionId}", orderId,
            sessionId);

    private static void LogCommandError(ILogger logger, int sessionId) => logger
        .ForContext("EventId", LoggerEventId.CreateOrderCommandError)
        .Error("Error creating order from shopping session with ID {SessionId}", sessionId);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.CreateOrderDatabaseException)
        .Error(ex, "Database error occurred while creating order. Error: {ErrorMessage}", errorMessage);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) => logger
        .ForContext("EventId", LoggerEventId.CreateOrderDomainException)
        .Error(ex, "Invalid operation while creating order. Error: {ErrorMessage}", errorMessage);
}
