#region

using DemoShop.Domain.Common.ValueObjects;

#endregion

namespace DemoShop.TestUtils.Common.Customizations;

public class QuantityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() =>
            Quantity.Create(
                fixture.Create<int>()
            )
        );
}
