#region

using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Entities;

#endregion

namespace DemoShop.Domain.User.Events;

public class UserAddressUpdatedDomainEvent(int id, AddressEntity newAddress, AddressEntity? oldAddress) : IDomainEvent
{
    public int Id { get; } = id;
    public AddressEntity NewAddress { get; } = newAddress;
    public AddressEntity? OldAddress { get; } = oldAddress;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
