using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.User.Entities;

public class AddressEntity : Entity
{
    private AddressEntity()
    {
        Street = string.Empty;
        Apartment = string.Empty;
        City = string.Empty;
        Zip = string.Empty;
        Country = string.Empty;
    }

    public AddressEntity(int userId, string street, string apartment, string city,
        string zip, string country, string? region)
    {
        UserId = userId;
        Street = Guard.Against.NullOrEmpty(street);
        Apartment = Guard.Against.NullOrEmpty(apartment);
        City = Guard.Against.NullOrEmpty(city);
        Zip = Guard.Against.NullOrEmpty(zip);
        Country = Guard.Against.NullOrEmpty(country);
        Region = region;
    }

    public int UserId { get; private init; }
    public string Street { get; private init; }
    public string Apartment { get; private init; }
    public string City { get; private init; }
    public string Zip { get; private init; }
    public string Country { get; private init; }
    public string? Region { get; private init; }

    public UserEntity? User { get; init; }
}
