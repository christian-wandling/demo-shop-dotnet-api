#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductImageDeletedHandler(ILogger<ProductImageDeletedHandler> logger)
    : INotificationHandler<ImageDeletedDomainEvent>
{
    public Task Handle(ImageDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        logger.LogOperationSuccess("Delete Image", "id", $"{notification.Id}");

        return Task.CompletedTask;
    }
}
