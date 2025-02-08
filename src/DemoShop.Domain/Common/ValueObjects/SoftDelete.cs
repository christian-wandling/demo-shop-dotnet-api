#region

using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Common.ValueObjects;

public sealed record SoftDelete : ValueObject
{
    private SoftDelete(DateTime? deletedAt = null)
    {
        DeletedAt = deletedAt;
    }

    public DateTime? DeletedAt { get; private set; }
    public bool Deleted { get; private set; }

    public static SoftDelete Create() => new();

    public void MarkAsDeleted()
    {
        DeletedAt = DateTime.UtcNow;
        Deleted = true;
    }

    public void Restore()
    {
        DeletedAt = null;
        Deleted = false;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DeletedAt ?? DateTime.MinValue;
    }
}
