#region

using AutoFixture.AutoNSubstitute;
using DemoShop.TestUtils.Common.Customizations;
using Xunit.Abstractions;

#endregion

namespace DemoShop.TestUtils.Common.Base;

public abstract class Test
{
    protected readonly IFixture Fixture;
    protected readonly ITestOutputHelper? Output;

    protected Test(ITestOutputHelper? output = null)
    {
        Output = output;

        Fixture = new Fixture()
            .Customize(new AutoNSubstituteCustomization { ConfigureMembers = true })
            .Customize(new CustomizationRegistration());

        Output = output;

        // Handle recursive references
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    protected T Mock<T>() where T : class => Fixture.Freeze<T>();
    protected T Create<T>() => Fixture.Create<T>();
}
