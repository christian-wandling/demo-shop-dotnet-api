using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.DTOs;

namespace DemoShop.Domain.User.Entities;

public sealed class AddressEntity : IEntity, IAggregateRoot
{
    private AddressEntity()
    {
        Street = string.Empty;
        Apartment = string.Empty;
        City = string.Empty;
        Zip = string.Empty;
        Country = string.Empty;
    }

    private AddressEntity(CreateAddressDto createAddress)
    {
        UserId = Guard.Against.NegativeOrZero(createAddress.UserId);
        Street = Guard.Against.NullOrEmpty(createAddress.Street);
        Apartment = Guard.Against.NullOrEmpty(createAddress.Apartment);
        City = Guard.Against.NullOrEmpty(createAddress.City);
        Zip = Guard.Against.NullOrEmpty(createAddress.Zip);
        Country = Guard.Against.NullOrEmpty(createAddress.Country);
        Region = createAddress.Region;
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    public int Id { get; }
    public string Street { get; private set; }
    public string Apartment { get; private set; }
    public string City { get; private set; }
    public string Zip { get; private set; }
    public string Country { get; private set; }
    public string? Region { get; private set; }
    public int UserId { get; private set; }
    public UserEntity User { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime ModifiedAt { get; set; }

    public static Result<AddressEntity> Create(CreateAddressDto createAddress)
    {
        Guard.Against.Null(createAddress, nameof(createAddress));
        var address = new AddressEntity(createAddress);
        return Result.Success(address);
    }

    public Result Update(UpdateAddressDto updateAddress)
    {
        Guard.Against.Null(updateAddress, nameof(updateAddress));

        Street = Guard.Against.NullOrEmpty(updateAddress.Street);
        Apartment = Guard.Against.NullOrEmpty(updateAddress.Apartment);
        City = Guard.Against.NullOrEmpty(updateAddress.City);
        Zip = Guard.Against.NullOrEmpty(updateAddress.Zip);
        Country = Guard.Against.NullOrEmpty(updateAddress.Country);
        Region = updateAddress.Region;
        ModifiedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
