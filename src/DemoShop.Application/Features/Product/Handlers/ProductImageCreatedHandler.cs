using Ardalis.GuardClauses;
using DemoShop.Application.Features.User.Handlers;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductImageCreatedHandler(ILogger<ProductImageCreatedHandler> logger)
    : INotificationHandler<ImageCreatedDomainEvent>
{
    public Task Handle(ImageCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.Image, nameof(notification.Image));

        logger.LogOperationSuccess("Create Image", "id", $"{notification.Image.Id}");

        return Task.CompletedTask;
    }
}
