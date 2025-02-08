#region

using DemoShop.Domain.ShoppingSession.Entities;

#endregion

namespace DemoShop.TestUtils.Features.ShoppingSession;

public class CartItemEntityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() =>
            CartItemEntity.Create(
                fixture.Create<int>(),
                fixture.Create<int>()
            ).Value
        );
}
