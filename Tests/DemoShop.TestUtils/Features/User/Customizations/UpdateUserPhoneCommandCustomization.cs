using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.User.Commands.UpdateUserPhone;

namespace DemoShop.TestUtils.Features.User.Customizations;

public class UpdateUserPhoneCommandCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() => new UpdateUserPhoneCommand(
               new UpdateUserPhoneRequest
               {
                   Phone = "+123456789"
               }
            )
        );
}
