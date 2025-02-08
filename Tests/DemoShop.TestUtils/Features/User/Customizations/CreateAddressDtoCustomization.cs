#region

using DemoShop.Domain.User.DTOs;

#endregion

namespace DemoShop.TestUtils.Features.User.Customizations;

public class CreateAddressDtoCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Customize<CreateAddressDto>(composer => composer
            .With(dto => dto.Street, fixture.Create<string>())
            .With(dto => dto.Apartment, fixture.Create<string>())
            .With(dto => dto.City, fixture.Create<string>())
            .With(dto => dto.Zip, fixture.Create<string>())
            .With(dto => dto.Region, fixture.Create<string>())
            .With(dto => dto.Country, fixture.Create<string>())
        );
}
