#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.Order.Commands.ConvertShoppingSessionToOrder;

public sealed class ConvertShoppingSessionToOrderCommandHandler(
    IOrderRepository repository,
    IDomainEventDispatcher eventDispatcher,
    ILogger<ConvertShoppingSessionToOrderCommandHandler> logger
)
    : IRequestHandler<ConvertShoppingSessionToOrderCommand, Result<OrderEntity>>
{
    public async Task<Result<OrderEntity>> Handle(ConvertShoppingSessionToOrderCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Session, nameof(request.Session));

        try
        {
            var unsavedResult = request.Session.ConvertToOrder();

            if (!unsavedResult.IsSuccess)
                return unsavedResult.Map();

            return await SaveChanges(unsavedResult.Value, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            logger.LogOperationFailed("Convert shopping session to order", "ShoppingSessionId", $"{request.Session.Id}",
                ex);
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
}
