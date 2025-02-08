#region

using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Entities;

#endregion

namespace DemoShop.Domain.User.Events;

public class UserCreatedDomainEvent(UserEntity user) : IDomainEvent
{
    public UserEntity User { get; } = user;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
