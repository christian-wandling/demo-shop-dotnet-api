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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
