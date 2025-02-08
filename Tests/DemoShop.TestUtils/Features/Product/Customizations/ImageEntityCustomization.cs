#region

using DemoShop.Domain.Product.DTOs;
using DemoShop.Domain.Product.Entities;

#endregion

namespace DemoShop.TestUtils.Features.Product.Customizations;

public class ImageEntityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() => ImageEntity.Create(
            fixture.Create<CreateImageDto>()
        ).Value);
}
