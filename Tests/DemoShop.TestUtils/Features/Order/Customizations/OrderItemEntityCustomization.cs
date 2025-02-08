#region

using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.ValueObjects;

#endregion

namespace DemoShop.TestUtils.Features.Order.Customizations;

public class OrderItemEntityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() =>
            OrderItemEntity.Create(
                fixture.Create<int>(),
                fixture.Create<OrderProduct>(),
                fixture.Create<Quantity>(),
                fixture.Create<Price>()
            ).Value
        );
}
