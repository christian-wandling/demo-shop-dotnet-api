#region

using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.User.Commands.UpdateUserAddress;

#endregion

namespace DemoShop.TestUtils.Features.User.Customizations;

public class UpdateUserAddressCommandCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() => new UpdateUserAddressCommand(
                new UpdateUserAddressRequest
                {
                    Street = "Street",
                    Apartment = "Apartment",
                    City = "City",
                    Zip = "Zip",
                    Country = "Country"
                }
            )
        );
}
