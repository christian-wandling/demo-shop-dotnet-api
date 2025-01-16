using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Domain.Common.Base;

public abstract class EntitySoftDelete : Entity, IEntitySoftDelete
{
    public bool Deleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    protected EntitySoftDelete() : base()
    {
        Deleted = false;
    }

    public virtual void Delete()
    {
        if (Deleted)
            return;

        Deleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public virtual void Restore()
    {
        if (!Deleted)
            return;

        Deleted = false;
        DeletedAt = null;
    }
}
