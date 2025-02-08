#region

using MediatR;

#endregion

namespace DemoShop.Domain.Common.Interfaces;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
