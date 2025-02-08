#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.Order.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(
    ICurrentUserAccessor user,
    IMapper mapper,
    ILogger<GetOrderByIdQueryHandler> logger,
    IOrderRepository repository
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

            if (!userIdResult.IsSuccess) return Result.Forbidden("Authorization Failed");

            var order = await repository.GetOrderByIdAsync(
                request.Id,
                userIdResult.Value,
                cancellationToken
            );

            if (order is not null)
                return Result.Success(mapper.Map<OrderResponse>(order));

            logger.LogOperationFailed("Get Order By Id", "Id", $"{request.Id}", null);
            return Result.NotFound($"Order with Id {request.Id} not found");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            logger.LogOperationFailed("Get order by Id", "Id", $"{request.Id}", ex);
            return Result.Error(ex.Message);
        }
    }
}
