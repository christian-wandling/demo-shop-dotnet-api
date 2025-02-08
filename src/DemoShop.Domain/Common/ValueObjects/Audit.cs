#region

using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Common.ValueObjects;

public sealed record Audit : ValueObject
{
    private Audit(DateTime createdAt, DateTime modifiedAt)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; private set; }

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
