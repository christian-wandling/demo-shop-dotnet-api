using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.TestUtils.Features.User.Models;

namespace DemoShop.TestUtils.Features.User.Customizations;

public class CreateUserCommandCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() => new CreateUserCommand(
            fixture.Create<TestUserIdentity>()
        ));
}
