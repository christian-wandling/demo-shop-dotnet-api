#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductCreatedHandler(ILogger logger)
    : INotificationHandler<ProductCreatedDomainEvent>
{
    public Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.Product, nameof(notification.Product));

        LogProductCreated(logger, notification.Product.Id);
        return Task.CompletedTask;
    }

    private static void LogProductCreated(ILogger logger, int id) => logger.Information(
        "Product created: {Id} {@EventId}", id, LoggerEventIds.ProductCreatedDomainEvent);
}
