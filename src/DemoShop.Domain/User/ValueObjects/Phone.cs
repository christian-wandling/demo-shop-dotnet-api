#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.User.ValueObjects;

public sealed record Phone : ValueObject
{
    private Phone()
    {
        Value = null;
    }

    private Phone(string? value)
    {
        Value = Guard.Against.InvalidPhone(value, nameof(value));
    }

    public string? Value { get; }

    public static Phone Empty => new();

    public static Phone Create(string? value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value ?? string.Empty;
    }
}
