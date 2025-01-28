using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.User.ValueObjects;

public sealed record KeycloakUserId : ValueObject
{
    public string Value { get; private set; }

    private KeycloakUserId()
    {
        Value = string.Empty;
    }

    private KeycloakUserId(string value)
    {
        Value = Guard.Against.NullOrWhiteSpace(value, nameof(value));
    }

    public static KeycloakUserId Empty => new();

    public static KeycloakUserId Create(string id) => new(id);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
