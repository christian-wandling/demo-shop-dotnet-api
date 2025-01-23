using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.User.ValueObjects;

public sealed record EmailAddress : ValueObject
{
    public string Value { get; private set; }

    private EmailAddress(string value)
    {
        Value = Guard.Against.InvalidEmail(value, nameof(value));
    }

    public static EmailAddress Empty => new(string.Empty);

    public static EmailAddress Create(string email) => new(email);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
