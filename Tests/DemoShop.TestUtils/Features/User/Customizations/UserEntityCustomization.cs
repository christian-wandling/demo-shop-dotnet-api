#region

using DemoShop.Domain.User.Entities;
using DemoShop.TestUtils.Features.User.Models;

#endregion

namespace DemoShop.TestUtils.Features.User.Customizations;

public class UserEntityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Register(() => UserEntity.Create(
            fixture.Create<TestUserIdentity>()
        ).Value);
}
