#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.User.ValueObjects;

public sealed record KeycloakUserId : ValueObject
{
    private KeycloakUserId()
    {
        Value = string.Empty;
    }

    private KeycloakUserId(string value)
    {
        Value = Guard.Against.NullOrWhiteSpace(value, nameof(value));
    }

    public string Value { get; }

    public static KeycloakUserId Empty => new();

    public static KeycloakUserId Create(string id) => new(id);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
