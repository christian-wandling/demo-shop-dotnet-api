using DemoShop.Domain.Common.Base;
using DemoShop.Domain.User.Interfaces;

namespace DemoShop.Domain.User.ValueObjects;

public sealed record SoftDeleteAudit : ValueObject
{
    public DateTime? DeletedAt { get; private set; }
    public bool IsDeleted => DeletedAt.HasValue;

    private SoftDeleteAudit(DateTime? deletedAt = null)
    {
        DeletedAt = deletedAt;
    }

    public static SoftDeleteAudit Create() => new();

    public void MarkAsDeleted() => DeletedAt = DateTime.UtcNow;

    public void Restore() => DeletedAt = null;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DeletedAt ?? DateTime.MinValue;
    }
}
