using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Session.Entities;

namespace DemoShop.Domain.User.Entities;

public sealed class UserEntity : EntitySoftDelete, IAggregateRoot
{
    private readonly List<ShoppingSessionEntity> _shoppingSessions = [];
    private readonly List<Order.Entities.OrderEntity> _orders = [];

    private UserEntity()
    {
        KeycloakUserId = Guid.Empty;
        Email = string.Empty;
        Firstname = string.Empty;
        Lastname = string.Empty;
    }

    private UserEntity(Guid keycloakUserId, string email, string firstname, string lastname)
    {
        KeycloakUserId = Guard.Against.Default(keycloakUserId, nameof(keycloakUserId));
        Email = Guard.Against.NullOrEmpty(email, nameof(email));
        Firstname = Guard.Against.NullOrEmpty(firstname, nameof(firstname));
        Lastname = Guard.Against.NullOrEmpty(lastname, nameof(lastname));
    }

    public Guid KeycloakUserId { get; private init; }
    public string Email { get; private init; }
    public string Firstname { get; private set; }
    public string Lastname { get; private set; }
    public string? Phone { get; private set; }
    public AddressEntity? Address { get; private set; }

    public IReadOnlyCollection<ShoppingSessionEntity> ShoppingSessions => _shoppingSessions.AsReadOnly();
    public IReadOnlyCollection<Order.Entities.OrderEntity> Orders => _orders.AsReadOnly();

    public static Result<UserEntity> Create(Guid keycloakUserId, string email, string firstname, string lastname)
    {
        var user = new UserEntity(keycloakUserId, email, firstname, lastname);
        return Result.Success(user);
    }

    public Result<string?> UpdatePhone(string? phone)
    {
        Phone = phone;
        return Result.Success(phone);
    }

    public Result<AddressEntity> UpdateAddress(string street, string apartment, string city,
        string zip, string country, string? region = null)
    {
        Address = new AddressEntity(Id, street, apartment, city, zip, country, region);
        return Result.Success(Address);
    }
}
