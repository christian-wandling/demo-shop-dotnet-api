using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.User.ValueObjects;

public sealed record SoftDelete : ValueObject
{
    public DateTime? DeletedAt { get; private set; }
    public bool Deleted { get; private set; }

    private SoftDelete(DateTime? deletedAt = null)
    {
        DeletedAt = deletedAt;
    }

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
