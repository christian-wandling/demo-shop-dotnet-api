using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    SoftDeleteAudit SoftDeleteAudit { get; }
}
