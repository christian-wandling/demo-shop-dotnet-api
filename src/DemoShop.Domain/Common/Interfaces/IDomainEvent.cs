using MediatR;

namespace DemoShop.Domain.Common.Interfaces;

public interface IDomainEvent: INotification
{
    DateTime OccurredOn { get; }
}
