#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Product.DTOs;
using DemoShop.Domain.Product.Events;

#endregion

namespace DemoShop.Domain.Product.Entities;

public class ImageEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private ImageEntity()
    {
        Name = string.Empty;
        Uri = new Uri("about:blank");
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    private ImageEntity(string name, Uri uri)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Uri = Guard.Against.InvalidInput(uri, nameof(uri),
            u => Uri.TryCreate(u.ToString(), UriKind.Absolute, out _),
            "URI must be a valid absolute URI.");
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    public string Name { get; private set; }
    public Uri Uri { get; private set; }
    public int? ProductId { get; private set; }
    public ProductEntity? Product { get; private set; }
    public Audit Audit { get; }

    public int Id { get; }
    public SoftDelete SoftDelete { get; }

    public static Result<ImageEntity> Create(CreateImageDto createImage)
    {
        Guard.Against.Null(createImage, nameof(createImage));

        var image = new ImageEntity(createImage.Name, createImage.Uri);
        image.AddDomainEvent(new ProductImageCreatedDomainEvent(image));
        return Result.Success(image);
    }
}
