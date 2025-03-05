#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductImageCreatedHandler(ILogger logger)
    : INotificationHandler<ProductImageCreatedDomainEvent>
{
    public Task Handle(ProductImageCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        LogProductImageCreated(logger, notification.Image.Id);
        return Task.CompletedTask;
    }

    private static void LogProductImageCreated(ILogger logger, int id) => logger
        .ForContext("EventId", LoggerEventId.ProductImageCreatedDomainEvent)
        .Information("Product image created: {Id}", id);
}
