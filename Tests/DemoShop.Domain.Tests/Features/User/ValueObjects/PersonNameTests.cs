#region

using DemoShop.Domain.User.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.User.ValueObjects;

[Trait("Feature", "User")]
public class PersonNameTests : Test
{
    [Fact]
    public void Empty_ShouldReturnEmptyPersonName()
    {
        // Act
        var personName = PersonName.Empty;

        // Assert
        personName.Should().NotBeNull();
        personName.Firstname.Should().BeEmpty();
        personName.Lastname.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "Smith")]
    [InlineData(" ", "Smith")]
    [InlineData(null, "Smith")]
    [InlineData("John", "")]
    [InlineData("John", " ")]
    [InlineData("John", null)]
    public void Create_WithInvalidInput_ShouldThrowArgumentException(string firstname, string lastname)
    {
        // Act
        var action = () => PersonName.Create(firstname, lastname);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithValidInput_ShouldCreatePersonName()
    {
        // Arrange
        const string firstname = "John";
        const string lastname = "Smith";

        // Act
        var personName = PersonName.Create(firstname, lastname);

        // Assert
        personName.Should().NotBeNull();
        personName.Firstname.Should().Be(firstname);
        personName.Lastname.Should().Be(lastname);
    }

    [Fact]
    public void Equal_PersonNames_ShouldBeEqual()
    {
        // Arrange
        var personName1 = PersonName.Create("John", "Smith");
        var personName2 = PersonName.Create("John", "Smith");

        // Assert
        personName1.Should().Be(personName2);
        personName1.GetHashCode().Should().Be(personName2.GetHashCode());
    }

    [Theory]
    [InlineData("John", "Smith", "Jane", "Smith")]
    [InlineData("John", "Smith", "John", "Doe")]
    public void Different_PersonNames_ShouldNotBeEqual(
        string firstname1, string lastname1,
        string firstname2, string lastname2)
    {
        // Arrange
        var personName1 = PersonName.Create(firstname1, lastname1);
        var personName2 = PersonName.Create(firstname2, lastname2);

        // Assert
        personName1.Should().NotBe(personName2);
        personName1.GetHashCode().Should().NotBe(personName2.GetHashCode());
    }

    [Fact]
    public void Equal_PersonNames_ShouldHaveSameHashCode()
    {
        // Arrange
        var personName1 = PersonName.Create("John", "Smith");
        var personName2 = PersonName.Create("John", "Smith");

        // Act
        var hashCode1 = personName1.GetHashCode();
        var hashCode2 = personName2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }
}
