#region

using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.ValueObjects;

#endregion

namespace DemoShop.Domain.User.Events;

public class UserPhoneUpdatedDomainEvent(int id, Phone newPhone, Phone oldPhone) : IDomainEvent
{
    public int Id { get; } = id;
    public Phone NewPhone { get; } = newPhone;
    public Phone OldPhone { get; } = oldPhone;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
