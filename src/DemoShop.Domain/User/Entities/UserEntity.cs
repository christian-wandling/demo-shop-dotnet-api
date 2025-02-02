using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Exceptions;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Events;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Events;
using DemoShop.Domain.User.DTOs;
using DemoShop.Domain.User.Events;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.User.Entities;

public sealed class UserEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private readonly List<ShoppingSessionEntity> _shoppingSessions = [];
    private readonly List<OrderEntity> _orders = [];

    private UserEntity()
    {
        KeycloakUserId = KeycloakUserId.Empty;
        Email = EmailAddress.Empty;
        PersonName = PersonName.Empty;
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    private UserEntity(IUserIdentity userIdentity)
    {
        KeycloakUserId = KeycloakUserId.Create(userIdentity.KeycloakUserId);
        Email = EmailAddress.Create(userIdentity.Email);
        PersonName = PersonName.Create(userIdentity.FirstName, userIdentity.LastName);
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    public int Id { get; }
    public KeycloakUserId KeycloakUserId { get; private init; }
    public EmailAddress Email { get; private init; }
    public PersonName PersonName { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public AddressEntity? Address { get; private set; }
    public Audit Audit { get; }
    public SoftDelete SoftDelete { get; }
    public IReadOnlyCollection<OrderEntity> Orders => _orders.AsReadOnly();
    public IReadOnlyCollection<ShoppingSessionEntity> ShoppingSessions => _shoppingSessions.AsReadOnly();

    public static UserEntity Create(IUserIdentity userIdentity)
    {
        Guard.Against.Null(userIdentity, nameof(userIdentity));

        var user = new UserEntity(userIdentity);
        user.AddDomainEvent(new UserCreatedDomainEvent(user));
        return user;
    }

    public void UpdatePhone(string? phone)
    {
        var oldPhone = Phone;
        Phone = PhoneNumber.Create(phone);
        Audit.UpdateModified();

        this.AddDomainEvent(new UserPhoneUpdatedDomainEvent(Id, Phone, oldPhone));
    }

    public void SetInitialAddress(CreateAddressDto createAddress)
    {
        if (Address != null)
            throw new InvalidOperationException(
                "Address already set. Use UpdateAddress to modify the existing address."
            );

        var address = AddressEntity.Create(createAddress);

        Guard.Against.Null(address, nameof(address));

        Address = address;
        Audit.UpdateModified();

        this.AddDomainEvent(new UserAddressUpdatedDomainEvent(Id, Address, null));
    }

    public void UpdateAddress(UpdateAddressDto updateAddress)
    {
        if (Address == null)
            throw new InvalidOperationException(
                "Address not found. Use SetInitalAddress to create an address."
            );

        var oldAddress = Address;
        Address.Update(updateAddress);
        Audit.UpdateModified();
        this.AddDomainEvent(new UserAddressUpdatedDomainEvent(Id, Address, oldAddress));
    }
}
