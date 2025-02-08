#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Common.ValueObjects;

public sealed record Quantity : ValueObject
{
    private Quantity()
    {
        Value = 0;
    }

    private Quantity(int value)
    {
        Value = Guard.Against.NegativeOrZero(value);
    }

    public int Value { get; }

    public static Quantity Create(int value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
