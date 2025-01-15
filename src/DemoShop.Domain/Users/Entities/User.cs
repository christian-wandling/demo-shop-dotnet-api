using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Orders.Entities;
using DemoShop.Domain.Sessions.Entities;

namespace DemoShop.Domain.Users.Entities;

public sealed class User : EntitySoftDelete, IAggregateRoot
{
    private readonly List<ShoppingSession> _shoppingSessions = [];
    private readonly List<Order> _orders = [];

    private User()
    {
        KeycloakUserId = Guid.Empty;
        Email = string.Empty;
        Firstname = string.Empty;
        Lastname = string.Empty;
    }

    private User(Guid keycloakUserId, string email, string firstname, string lastname)
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
    public Address? Address { get; private set; }

    public IReadOnlyCollection<ShoppingSession> ShoppingSessions => _shoppingSessions.AsReadOnly();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    public static Result<User> Create(Guid keycloakUserId, string email, string firstname, string lastname)
    {
        var user = new User(keycloakUserId, email, firstname, lastname);
        return Result.Success(user);
    }

    public Result<string?> UpdatePhone(string? phone)
    {
        Phone = phone;
        return Result.Success(phone);
    }

    public Result<Address> UpdateAddress(string street, string apartment, string city,
        string zip, string country, string? region = null)
    {
        Address = new Address(Id, street, apartment, city, zip, country, region);
        return Result.Success(Address);
    }
}
