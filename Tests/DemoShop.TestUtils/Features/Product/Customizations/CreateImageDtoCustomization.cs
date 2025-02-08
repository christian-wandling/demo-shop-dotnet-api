#region

using DemoShop.Domain.Product.DTOs;

#endregion

namespace DemoShop.TestUtils.Features.Product.Customizations;

public class CreateImageDtoCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() => new CreateImageDto { Name = fixture.Create<string>(), Uri = fixture.Create<Uri>() });
}
