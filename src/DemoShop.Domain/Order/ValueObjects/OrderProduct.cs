#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Order.ValueObjects;

public sealed record OrderProduct : ValueObject
{
    private OrderProduct()
    {
        ProductName = string.Empty;
        ProductThumbnail = string.Empty;
    }

    private OrderProduct(string productName, string productThumbnail)
    {
        ProductName = Guard.Against.NullOrWhiteSpace(productName, nameof(productName));
        ProductThumbnail = productThumbnail;
    }

    public string ProductName { get; }
    public string? ProductThumbnail { get; }

    public static OrderProduct Empty => new();

    public static OrderProduct Create(string productName, string productThumbnail) =>
        new(productName, productThumbnail);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductName;
        yield return ProductThumbnail ?? string.Empty;
    }
}
