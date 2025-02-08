#region

using DemoShop.Domain.Product.Entities;

#endregion

namespace DemoShop.TestUtils.Features.Product.Customizations;

public class CategoryEntityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() => CategoryEntity.Create(
            fixture.Create<string>()
        ).Value);
}
