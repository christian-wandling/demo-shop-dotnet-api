#region

using DemoShop.TestUtils.Features.User.Models;

#endregion

namespace DemoShop.TestUtils.Common.Customizations;

public class UserIdentityCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Customize<TestUserIdentity>(composer => composer
            .With(dto => dto.KeycloakUserId, fixture.Create<string>())
            .With(dto => dto.FirstName, fixture.Create<string>())
            .With(dto => dto.LastName, fixture.Create<string>())
            .With(dto => dto.Email, "test@example.com")
        );
}
