#region

using Ardalis.GuardClauses;
using DemoShop.Domain.User.Exceptions;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.User.Guards;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
[Trait("Feature", "User")]
public class InvalidEmailGuardTests : Test
{
    [Fact]
    public void InvalidEmail_WithValidEmail_ReturnsInput()
    {
        // Arrange
        const string validEmail = "test@example.com";

        // Act
        var result = Guard.Against.InvalidEmail(validEmail);

        // Assert
        result.Should().Be(validEmail);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void InvalidEmail_WithNullOrWhiteSpace_ThrowsArgumentNullException(string invalidEmail)
    {
        // Act & Assert
        var act = () => Guard.Against.InvalidEmail(invalidEmail);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@nodomain")]
    [InlineData("missing@.com")]
    [InlineData("space in@email.com")]
    [InlineData("multiple@@at.com")]
    public void InvalidEmail_WithInvalidFormat_ThrowsInvalidEmailDomainException(string invalidEmail)
    {
        // Act & Assert
        var act = () => Guard.Against.InvalidEmail(invalidEmail);

        act.Should().Throw<InvalidEmailDomainException>()
            .WithMessage($"Email '{invalidEmail}' is not in a valid format");
    }

    [Theory]
    [InlineData("simple@example.com")]
    [InlineData("very.common@example.com")]
    [InlineData("disposable.style.email.with+symbol@example.com")]
    [InlineData("other.email-with-hyphen@example.com")]
    [InlineData("fully-qualified-domain@example.com")]
    [InlineData("user.name+tag+sorting@example.com")]
    [InlineData("x@example.com")]
    [InlineData("example-indeed@strange-example.com")]
    public void InvalidEmail_WithValidEmailFormats_ReturnsInput(string validEmail)
    {
        // Act
        var result = Guard.Against.InvalidEmail(validEmail);

        // Assert
        result.Should().Be(validEmail);
    }

    [Fact]
    public void InvalidEmail_WithDifferentCaseInMailAddress_ReturnsInput()
    {
        // Arrange
        const string email = "Test.Email@Example.COM";

        // Act
        var result = Guard.Against.InvalidEmail(email);

        // Assert
        result.Should().Be(email);
    }
}
