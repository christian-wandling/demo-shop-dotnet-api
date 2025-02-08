#region

using DemoShop.Domain.Order.Entities;

#endregion

namespace DemoShop.TestUtils.Features.Order.Customizations;

public class OrderEntityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() =>
            OrderEntity.Create(
                fixture.Create<int>(),
                fixture.CreateMany<OrderItemEntity>(3).ToList()
            ).Value
        );
}
