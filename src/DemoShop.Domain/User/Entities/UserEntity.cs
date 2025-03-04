#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.User.DTOs;
using DemoShop.Domain.User.Events;
using DemoShop.Domain.User.ValueObjects;

#endregion

namespace DemoShop.Domain.User.Entities;

public sealed class UserEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private readonly List<OrderEntity> _orders = [];
    private readonly List<ShoppingSessionEntity> _shoppingSessions = [];

    private UserEntity()
    {
        KeycloakUserId = KeycloakUserId.Empty;
        Email = Email.Empty;
        Phone = Phone.Empty;
        PersonName = PersonName.Empty;
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    private UserEntity(IUserIdentity userIdentity)
    {
        KeycloakUserId = KeycloakUserId.Create(userIdentity.KeycloakUserId);
        Email = Email.Create(userIdentity.Email);
        Phone = Phone.Create(null);
        PersonName = PersonName.Create(userIdentity.FirstName, userIdentity.LastName);
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    public KeycloakUserId KeycloakUserId { get; private init; }
    public Email Email { get; private init; }
    public PersonName PersonName { get; private set; }
    public Phone Phone { get; private set; }
    public AddressEntity? Address { get; private set; }
    public IReadOnlyCollection<OrderEntity> Orders => _orders.AsReadOnly();
    public IReadOnlyCollection<ShoppingSessionEntity> ShoppingSessions => _shoppingSessions.AsReadOnly();
    public Audit Audit { get; }

    public int Id { get; }
    public SoftDelete SoftDelete { get; }

    public static Result<UserEntity> Create(IUserIdentity userIdentity)
    {
        Guard.Against.Null(userIdentity, nameof(userIdentity));

        var user = new UserEntity(userIdentity);
        user.AddDomainEvent(new UserCreatedDomainEvent(user));

        return Result.Success(user);
    }

    public Result UpdatePhone(string? phone)
    {
        var oldPhone = Phone;
        Phone = Phone.Create(phone);
        Audit.UpdateModified();
        this.AddDomainEvent(new UserPhoneUpdatedDomainEvent(Id, KeycloakUserId.Value, Phone, oldPhone));

        return Result.Success();
    }

    public Result SetInitialAddress(CreateAddressDto createAddress)
    {
        if (Address != null)
            throw new InvalidOperationException(
                "Address already set. Use UpdateAddress to modify the existing address.");

        var result = AddressEntity.Create(createAddress);

        if (!result.IsSuccess) return result.Map();

        Address = result.Value;
        Audit.UpdateModified();

        this.AddDomainEvent(new UserAddressUpdatedDomainEvent(Id, KeycloakUserId.Value, Address, null));

        return Result.Success();
    }

    public Result UpdateAddress(UpdateAddressDto updateAddress)
    {
        if (Address == null)
            throw new InvalidOperationException(
                "Address not found. Use SetInitalAddress to create an address."
            );

        var oldAddress = Address;
        var result = Address.Update(updateAddress);

        if (!result.IsSuccess) return result.Map();

        Audit.UpdateModified();
        this.AddDomainEvent(new UserAddressUpdatedDomainEvent(Id, KeycloakUserId.Value, Address, oldAddress));

        return Result.Success();
    }
}
