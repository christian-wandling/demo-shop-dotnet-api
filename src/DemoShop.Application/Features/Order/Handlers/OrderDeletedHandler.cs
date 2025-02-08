#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Events;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.Order.Handlers;

public class OrderDeletedHandler(ILogger<OrderDeletedHandler> logger)
    : INotificationHandler<OrderDeletedDomainEvent>
{
    public Task Handle(OrderDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        logger.LogOperationSuccess("Delete Order", "id", $"{notification.Id}");
        return Task.CompletedTask;
    }
}
