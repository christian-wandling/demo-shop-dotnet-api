namespace DemoShop.Api.Common.Base;

public abstract record Request
{
    public virtual bool Equals(Request? other)
    {
        if (other is null) return false;
        return GetType() == other.GetType();
    }

    public override int GetHashCode() => GetType().GetHashCode();
}
