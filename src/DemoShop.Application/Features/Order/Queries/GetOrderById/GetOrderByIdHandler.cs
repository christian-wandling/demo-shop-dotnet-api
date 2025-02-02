using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Order.Queries.GetOrderById;

public sealed class GetOrderByIdHandler(
    ICurrentUserAccessor user,
    IMapper mapper,
    ILogger<GetOrderByIdHandler> logger,
    IOrderRepository repository
)
    : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse?>>
{
    public async Task<Result<OrderResponse?>> Handle(GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var userIdResult = await user.GetId(cancellationToken).ConfigureAwait(false);

        if (!userIdResult.IsSuccess) return Result.Forbidden("Authorization Failed");

        var order = await repository.GetOrderByIdAsync(
                request.Id,
                userIdResult.Value,
                cancellationToken
            )
            .ConfigureAwait(false);

        if (order is not null)
            return Result.Success(mapper.Map<OrderResponse?>(order));

        logger.LogOperationFailed("Get ORder By Id", "Id", $"{request.Id}", null);
        return Result.NotFound();
    }
}
