#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Events;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.Order.Handlers;

public class OrderItemDeletedHandler(ILogger<OrderItemDeletedHandler> logger)
    : INotificationHandler<OrderItemDeletedDomainEvent>
{
    public Task Handle(OrderItemDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        logger.LogOperationSuccess("Delete OrderItem", "id", $"{notification.Id}");
        return Task.CompletedTask;
    }
}
