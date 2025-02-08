#region

using DemoShop.Domain.Common.ValueObjects;

#endregion

namespace DemoShop.TestUtils.Common.Customizations;

public class PriceCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() =>
            Price.Create(
                fixture.Create<decimal>()
            )
        );
}
