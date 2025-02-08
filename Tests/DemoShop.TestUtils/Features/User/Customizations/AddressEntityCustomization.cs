#region

using DemoShop.Domain.User.DTOs;
using DemoShop.Domain.User.Entities;

#endregion

namespace DemoShop.TestUtils.Features.User.Customizations;

public class AddressEntityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() => AddressEntity.Create(
            fixture.Create<CreateAddressDto>()
        ).Value);
}
