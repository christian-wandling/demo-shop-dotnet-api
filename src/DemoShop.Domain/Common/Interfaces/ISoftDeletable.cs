namespace DemoShop.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    bool Deleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
