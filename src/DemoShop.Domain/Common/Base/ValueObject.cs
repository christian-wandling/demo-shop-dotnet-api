namespace DemoShop.Domain.Common.Base;

public abstract record ValueObject
{
    public virtual bool Equals(ValueObject? other) =>
        other != null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(x => x.GetHashCode())
            .Aggregate((x, y) => x ^ y);
}
