using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.User.ValueObjects;

public sealed record PhoneNumber : ValueObject
{
    public string? Value { get; private set; }

    private PhoneNumber()
    {
        Value = null;
    }

    private PhoneNumber(string? value)
    {
        Value = Guard.Against.InvalidPhone(value, nameof(value));
    }

    public static PhoneNumber Create(string? phoneNumber) => new(phoneNumber);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value ?? string.Empty;
    }
}
