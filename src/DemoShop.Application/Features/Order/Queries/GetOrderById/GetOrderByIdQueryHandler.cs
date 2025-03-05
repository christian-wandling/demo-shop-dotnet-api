#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Interfaces;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(
    ICurrentUserAccessor user,
    IMapper mapper,
    ILogger logger,
    IOrderRepository repository,
    ICacheService cacheService
)
    : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NegativeOrZero(request.Id, nameof(request.Id));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        try
        {
            var userIdResult = await user.GetId(cancellationToken);
            if (!userIdResult.IsSuccess)
                return Result.Forbidden("Authorization Failed");

            LogQueryStarted(logger, request.Id, userIdResult);

            var cacheKey = cacheService.GenerateCacheKey("order", request);
            var response = cacheService.GetFromCache<OrderResponse>(cacheKey)
                           ?? await GetFromDatabase(request.Id, userIdResult.Value, cacheKey, cancellationToken);

            if (response is null)
            {
                LogNotFound(logger, request.Id, userIdResult.Value);
                return Result.NotFound($"Order with Id {request.Id} not found");
            }

            LogQuerySuccess(logger, request.Id, userIdResult.Value);
            return Result.Success(response);
        }
        catch (InvalidOperationException ex)
        {
            LogInvalidOperationException(logger, request.Id, ex.Message, ex);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            LogDatabaseException(logger, request.Id, ex.Message, ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<OrderResponse?> GetFromDatabase(
        int orderId,
        int userId,
        string cacheKey,
        CancellationToken cancellationToken
    )
    {
        var order = await repository.GetOrderByIdAsync(orderId, userId, cancellationToken);

        if (order is null)
            return null;

        var response = mapper.Map<OrderResponse>(order);
        cacheService.SetCache(cacheKey, response);

        return response;
    }

    private static void LogQueryStarted(ILogger logger, int orderId, int userId) =>
        logger.ForContext("EventId", LoggerEventId.GetOrderByIdQueryStarted)
            .Debug("Starting query to retrieve order with ID {OrderId} for user {UserId}", orderId, userId);

    private static void LogQuerySuccess(ILogger logger, int orderId, int userId) =>
        logger.ForContext("EventId", LoggerEventId.GetOrderByIdQuerySuccess)
            .Information("Successfully retrieved order with ID {OrderId} for user {UserId}", orderId, userId);

    private static void LogNotFound(ILogger logger, int orderId, int userId) =>
        logger.ForContext("EventId", LoggerEventId.GetOrderByIdQueryNotFound)
            .Information("Order with ID {OrderId} for user {UserId} was not found", orderId, userId);

    private static void LogDatabaseException(ILogger logger, int orderId, string errorMessage, Exception ex) =>
        logger.ForContext("EventId", LoggerEventId.GetOrderByIdDatabaseException)
            .Error(ex, "Database error occurred while retrieving order with ID {OrderId}. Error: {ErrorMessage}",
                orderId, errorMessage);

    private static void
        LogInvalidOperationException(ILogger logger, int orderId, string errorMessage, Exception ex) =>
        logger.ForContext("EventId", LoggerEventId.GetOrderByIdDomainException)
            .Error(ex, "Invalid operation while retrieving order with ID {OrderId}. Error: {ErrorMessage}",
                orderId, errorMessage);
}
