using DemoShop.Domain.Common.Base;
using DemoShop.Domain.User.Interfaces;

namespace DemoShop.Domain.User.ValueObjects;

public sealed record Audit : ValueObject
{
    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; private set; }

    private Audit(DateTime createdAt, DateTime modifiedAt)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public static Audit Create()
    {
        var now = DateTime.UtcNow;
        return new Audit(now, now);
    }

    public void UpdateModified() => ModifiedAt = DateTime.UtcNow;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CreatedAt;
        yield return ModifiedAt;
    }
}
