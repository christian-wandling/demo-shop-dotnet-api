#region

using DemoShop.Domain.Product.Entities;

#endregion

namespace DemoShop.TestUtils.Features.Product.Customizations;

public class ProductEntityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() => ProductEntity.Create(
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<decimal>()
            ).Value
        );
}
