#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Common.ValueObjects;

public sealed record Price : ValueObject
{
    private Price()
    {
        Value = 0;
    }

    private Price(decimal value)
    {
        Value = Guard.Against.InvalidPrice(value, nameof(value));
    }

    public decimal Value { get; }

    public static Price Empty => new();

    public static Price Create(decimal price) => new(price);

    public Price Multiply(int multiplier)
    {
        var rounded = Math.Round(Value * multiplier, 2, MidpointRounding.AwayFromZero);

        return new Price(rounded);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
