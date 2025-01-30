using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.Common.ValueObjects;

public sealed record Quantity : ValueObject
{
    public int Value { get; private set; }

    private Quantity()
    {
        Value = 0;
    }

    private Quantity(int value)
    {
        Value = Guard.Against.NegativeOrZero(value);
    }

    public static Quantity Create(int value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
