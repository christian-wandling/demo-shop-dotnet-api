namespace DemoShop.TestUtils.Common.Customizations;

public class IntCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Customize<int>(composer => composer
            .FromFactory(() => new Random().Next(1, 101)));
}
