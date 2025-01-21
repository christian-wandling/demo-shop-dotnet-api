using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Session.Entities;
using DemoShop.Domain.User.Events;

namespace DemoShop.Domain.User.Entities;

public sealed class UserEntity : IEntity, ISoftDeletable, IAggregateRoot
{
    private readonly List<ShoppingSessionEntity> _shoppingSessions = [];
    private readonly List<OrderEntity> _orders = [];

    private UserEntity()
    {
        KeycloakUserId = Guid.Empty;
        Email = string.Empty;
        Firstname = string.Empty;
        Lastname = string.Empty;
    }

    private UserEntity(IUserIdentity userIdentity)
    {
        KeycloakUserId = Guard.Against.Default(userIdentity.KeycloakId, nameof(userIdentity.KeycloakId));
        Email = Guard.Against.InvalidEmail(userIdentity.Email, nameof(userIdentity.Email));
        Firstname = Guard.Against.NullOrWhiteSpace(userIdentity.FirstName, nameof(userIdentity.FirstName));
        Lastname = Guard.Against.NullOrWhiteSpace(userIdentity.LastName, nameof(userIdentity.LastName));
    }

    public int Id { get; }
    public Guid KeycloakUserId { get; private init; }
    public string Email { get; private init; }
    public string Firstname { get; private set; }
    public string Lastname { get; private set; }
    public string? Phone { get; private set; }
    public AddressEntity? Address { get; private set; }
    public IReadOnlyCollection<ShoppingSessionEntity> ShoppingSessions => _shoppingSessions.AsReadOnly();
    public IReadOnlyCollection<OrderEntity> Orders => _orders.AsReadOnly();
    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; }
    public DateTime? DeletedAt { get; set; }
    public bool Deleted { get; set; }

    public static Result<UserEntity> Create(IUserIdentity userIdentity)
    {
        Guard.Against.Null(userIdentity, nameof(userIdentity));

        var user = new UserEntity(userIdentity);
        user.AddDomainEvent(new UserCreatedDomainEvent(user));
        return Result.Success(user);
    }

    public Result<string> UpdatePhone(string phone)
    {
        Guard.Against.NullOrWhiteSpace(phone, nameof(phone));

        Phone = phone;
        return Result.Success();
    }

    // public Result<AddressEntity> SetInitialAddress(string street, string apartment, string city, string zip,
    //     string country, string? region = null)
    // {
    //     if (Address != null)
    //         return Result.Error("An address already exists. Use UpdateAddress to modify the existing address.");
    //
    //     var addressResult = AddressEntity.Create(Id, street, apartment, city, zip, country, region);
    //     if (addressResult.IsError())
    //         return addressResult;
    //
    //     Address = addressResult.Value;
    //     return Result.Success(Address);
    // }
    //
    // public Result<AddressEntity> UpdateAddress(string street, string apartment, string city, string zip, string country,
    //     string? region = null)
    // {
    //     if (Address == null)
    //         return Result.Error("No existing address to update. Use AddAddress to add an address.");
    //
    //     var updatedAddressResult = AddressEntity.Create(Id, street, apartment, city, zip, country, region);
    //     if (updatedAddressResult.IsError())
    //         return updatedAddressResult;
    //
    //     Address = updatedAddressResult.Value;
    //     return Result.Success(Address);
    // }
}
