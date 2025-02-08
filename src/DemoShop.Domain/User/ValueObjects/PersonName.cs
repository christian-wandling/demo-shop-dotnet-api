#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.User.ValueObjects;

public sealed record PersonName : ValueObject
{
    private PersonName()
    {
        Firstname = string.Empty;
        Lastname = string.Empty;
    }

    private PersonName(string firstname, string lastname)
    {
        Firstname = Guard.Against.NullOrWhiteSpace(firstname);
        Lastname = Guard.Against.NullOrWhiteSpace(lastname);
    }

    public string Firstname { get; }
    public string Lastname { get; }

    public static PersonName Empty => new();

    public static PersonName Create(string firstname, string lastname) => new(firstname, lastname);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Firstname;
        yield return Lastname;
    }
}
