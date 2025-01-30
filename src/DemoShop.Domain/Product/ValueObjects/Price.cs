using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.Product.ValueObjects;

public sealed record Price : ValueObject
{
    public decimal Value { get; private set; }

    private Price()
    {
        Value = 0;
    }

    private Price(decimal value)
    {
        Value = Guard.Against.InvalidPrice(value, nameof(value));
    }

    public static Price Empty => new();

    public static Price Create(decimal price) => new(price);

    public Price Multiply(int multiplier)
    {
        var rounded = Math.Round(Value * multiplier, 2, MidpointRounding.AwayFromZero);

        return new Price(rounded);
    }

    public int ToInt() => (int)Math.Round(Value, 2, MidpointRounding.AwayFromZero);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
