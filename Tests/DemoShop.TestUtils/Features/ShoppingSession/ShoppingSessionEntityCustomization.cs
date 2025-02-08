#region

using DemoShop.Domain.ShoppingSession.Entities;

#endregion

namespace DemoShop.TestUtils.Features.ShoppingSession;

public class ShoppingSessionEntityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() =>
            ShoppingSessionEntity.Create(
                fixture.Create<int>()
            ).Value
        );
}
