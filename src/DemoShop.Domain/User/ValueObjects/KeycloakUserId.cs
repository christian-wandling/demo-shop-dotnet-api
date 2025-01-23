using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.User.ValueObjects;

public sealed record KeycloakUserId : ValueObject
{
    public Guid Value { get; private set; }

    private KeycloakUserId(Guid value)
    {
        Value = Guard.Against.Default(value, nameof(value));
    }

    public static KeycloakUserId Empty => new(Guid.Empty);

    public static KeycloakUserId Create(Guid id) => new(id);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
