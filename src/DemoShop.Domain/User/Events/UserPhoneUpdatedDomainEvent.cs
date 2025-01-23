using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.User.Events;

public class UserPhoneUpdatedDomainEvent(int id, PhoneNumber? newPhone,  PhoneNumber? oldPhone ) : IDomainEvent
{
    public int Id { get; } = id;
    public PhoneNumber? NewPhone { get; } = newPhone;
    public PhoneNumber? OldPhone { get; } = oldPhone;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
