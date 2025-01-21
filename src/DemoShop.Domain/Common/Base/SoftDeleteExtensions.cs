using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Domain.Common.Base;

public static class SoftDeletableExtensions
{
    public static void Delete(this ISoftDeletable entity)
    {
        Guard.Against.Null(entity, nameof(entity));

        if (entity.Deleted)
            return;

        entity.Deleted = true;
        entity.DeletedAt = DateTime.UtcNow;
    }

    public static void Restore(this ISoftDeletable entity)
    {
        Guard.Against.Null(entity, nameof(entity));

        if (!entity.Deleted)
            return;

        entity.Deleted = false;
        entity.DeletedAt = null;
    }
}
