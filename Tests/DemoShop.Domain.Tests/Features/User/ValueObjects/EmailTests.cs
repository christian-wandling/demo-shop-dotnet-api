#region

using DemoShop.Domain.User.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.User.ValueObjects;

[Trait("Feature", "User")]
public class EmailTests : Test
{
    [Fact]
    public void Empty_ShouldReturnEmptyEmail()
    {
        // Act
        var email = Email.Empty;

        // Assert
        email.Value.Should().BeEmpty();
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.com")]
    [InlineData("first.last@sub.domain.com")]
    public void Create_WithValidEmail_ShouldCreateEmail(string validEmail)
    {
        // Act
        var email = Email.Create(validEmail);

        // Assert
        email.Value.Should().Be(validEmail);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidEmail_ShouldThrowException(string invalidEmail)
    {
        // Act & Assert
        var action = () => Email.Create(invalidEmail);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equals_WithSameEmail_ShouldReturnTrue()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        // Act & Assert
        email1.Should().Be(email2);
    }

    [Fact]
    public void Equals_WithDifferentEmail_ShouldReturnFalse()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com");
        var email2 = Email.Create("test2@example.com");

        // Act & Assert
        email1.Should().NotBe(email2);
    }

    [Fact]
    public void GetHashCode_WithSameEmail_ShouldReturnSameHashCode()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        // Act
        var hashCode1 = email1.GetHashCode();
        var hashCode2 = email2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_WithDifferentEmail_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com");
        var email2 = Email.Create("test2@example.com");

        // Act
        var hashCode1 = email1.GetHashCode();
        var hashCode2 = email2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }
}
