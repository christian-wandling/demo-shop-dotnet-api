using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.User.ValueObjects;

public sealed record PersonName : ValueObject
{
    public string Firstname { get; private set; }
    public string Lastname { get; private set;}

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

    public static PersonName Empty => new();

    public static PersonName Create(string firstname, string lastname) => new(firstname, lastname);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Firstname;
        yield return Lastname;
    }
}
