#region

using DemoShop.Domain.Order.ValueObjects;

#endregion

namespace DemoShop.TestUtils.Features.Order.Customizations;

public class OrderProductCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() =>
            OrderProduct.Create(
                fixture.Create<string>(),
                fixture.Create<string>()
            )
        );
}
