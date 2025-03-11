#region

using DemoShop.Domain.User.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.User.ValueObjects;

[Trait("Feature", "User")]
public class PhoneTests : Test
{
    [Fact]
    public void Create_WithValidPhone_ReturnsSuccessResult()
    {
        // Arrange
        const string phoneNumber = "+1234567890";

        // Act
        var result = Phone.Create(phoneNumber);

        // Assert
        result.Value.Should().Be(phoneNumber);
    }

    [Fact]
    public void Create_WithNull_ReturnsSuccessResult()
    {
        // Act
        var result = Phone.Create(null);

        // Assert
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Equals_WithSamePhoneNumber_ReturnsTrue()
    {
        // Arrange
        var phone1 = Phone.Create("+1234567890");
        var phone2 = Phone.Create("+1234567890");

        // Assert
        phone1.Should().Be(phone2);
        phone1.GetHashCode().Should().Be(phone2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentPhoneNumbers_ReturnsFalse()
    {
        // Arrange
        var phone1 = Phone.Create("+1234567890");
        var phone2 = Phone.Create("+9876543210");

        // Assert
        phone1.Should().NotBe(phone2);
    }

    [Fact]
    public void Equals_WithNullValues_HandledCorrectly()
    {
        // Arrange
        var phone1 = Phone.Create(null);
        var phone2 = Phone.Create(null);
        var phone3 = Phone.Create("+1234567890");

        // Assert
        phone1.Should().Be(phone2);
        phone1.Should().NotBe(phone3);
        phone1.GetHashCode().Should().Be(phone2.GetHashCode());
    }

    [Fact]
    public void Equals_WithSamePhone_ShouldReturnTrue()
    {
        // Arrange
        var phone1 = Phone.Create("+123456789");
        var phone2 = Phone.Create("+123456789");

        // Act & Assert
        phone1.Should().Be(phone2);
    }

    [Fact]
    public void Equals_WithDifferentPhone_ShouldReturnFalse()
    {
        // Arrange
        var phone1 = Phone.Create("+123456789");
        var phone2 = Phone.Create("+987654321");

        // Act & Assert
        phone1.Should().NotBe(phone2);
    }

    [Fact]
    public void GetHashCode_WithSamePhone_ShouldReturnSameHashCode()
    {
        // Arrange
        var phone1 = Phone.Create("+123456789");
        var phone2 = Phone.Create("+123456789");

        // Act
        var hashCode1 = phone1.GetHashCode();
        var hashCode2 = phone2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_WithDifferentPhone_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var phone1 = Phone.Create("+123456789");
        var phone2 = Phone.Create("+987654321");

        // Act
        var hashCode1 = phone1.GetHashCode();
        var hashCode2 = phone2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }
}
