#region

using Ardalis.GuardClauses;
using DemoShop.Domain.User.Exceptions;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.User.Guards;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Feature", "User")]
public class PhoneGuardTests : Test
{
    [Theory]
    [InlineData(null)]
    public void InvalidPhone_WithNull_ReturnsNull(string? phoneNumber)
    {
        // Act
        var result = Guard.Against.InvalidPhone(phoneNumber);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("+12345678901")] // International format
    [InlineData("+1234")] // Minimum length with plus
    [InlineData("+123456789012345")] // Maximum length with plus
    [InlineData("01234567890")] // Starting with 0
    [InlineData("0123")] // Minimum length with 0
    public void InvalidPhone_WithValidFormat_ReturnsInput(string validPhone)
    {
        // Act
        var result = Guard.Against.InvalidPhone(validPhone);

        // Assert
        result.Should().Be(validPhone);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("abc")]
    [InlineData("+abc")] // Invalid characters after plus
    [InlineData("0abc")] // Invalid characters after zero
    [InlineData("+")] // Plus only
    [InlineData("+0")] // Plus with zero/ Double zero
    [InlineData("(123) 456-7890")] // Format not allowed
    [InlineData("+1-234-567-8900")] // Hyphens not allowed
    [InlineData("+44 123 456")] // Spaces not allowed
    public void InvalidPhone_WithInvalidFormat_ThrowsInvalidPhoneDomainException(string invalidPhone)
    {
        // Act
        var act = () => Guard.Against.InvalidPhone(invalidPhone);

        // Assert
        act.Should().Throw<InvalidPhoneDomainException>()
            .WithMessage($"Phone number '{invalidPhone}' is not in a valid format");
    }

    [Theory]
    [InlineData("+442071234567")] // UK format
    [InlineData("+33123456789")] // French format
    [InlineData("+493012345678")] // German format
    [InlineData("+819012345678")] // Japanese format
    public void InvalidPhone_WithInternationalFormats_ReturnsInput(string internationalPhone)
    {
        // Act
        var result = Guard.Against.InvalidPhone(internationalPhone);

        // Assert
        result.Should().Be(internationalPhone);
    }

    [Fact]
    public void InvalidPhone_WithParameterName_UsesProvidedParameterName()
    {
        // Arrange
        const string invalidPhone = "abc";
        const string parameterName = "phoneParameter";

        // Act
        var act = () => Guard.Against.InvalidPhone(invalidPhone, parameterName);

        // Assert
        act.Should().Throw<InvalidPhoneDomainException>()
            .WithMessage($"Phone number '{invalidPhone}' is not in a valid format");
    }
}
