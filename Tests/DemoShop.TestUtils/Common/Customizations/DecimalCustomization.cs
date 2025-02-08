namespace DemoShop.TestUtils.Common.Customizations;

public class DecimalCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Customize<decimal>(composer => composer
            .FromFactory(() => Math.Round((decimal)(new Random().NextDouble() * 100), 2)));
}
