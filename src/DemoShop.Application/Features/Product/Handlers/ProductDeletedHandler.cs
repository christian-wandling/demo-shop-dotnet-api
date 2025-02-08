#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductDeletedHandler(ILogger<ProductDeletedHandler> logger)
    : INotificationHandler<ProductDeletedDomainEvent>
{
    public Task Handle(ProductDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        logger.LogOperationSuccess("Delete Product", "id", $"{notification.Id}");
        return Task.CompletedTask;
    }
}
