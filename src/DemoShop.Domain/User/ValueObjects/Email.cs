#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.User.ValueObjects;

public sealed record Email : ValueObject
{
    private Email()
    {
        Value = string.Empty;
    }

    private Email(string value)
    {
        Value = Guard.Against.InvalidEmail(value, nameof(value));
    }

    public string Value { get; }

    public static Email Empty => new();

    public static Email Create(string email) => new(email);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
