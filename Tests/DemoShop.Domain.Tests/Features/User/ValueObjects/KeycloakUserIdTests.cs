#region

using DemoShop.Domain.User.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Features.User.ValueObjects;

[Trait("Feature", "User")]
public class KeycloakUserIdTests : Test
{
    [Fact]
    public void Empty_ShouldReturnEmptyKeycloakUserId()
    {
        // Act
        var userId = KeycloakUserId.Empty;

        // Assert
        userId.Value.Should().BeEmpty();
    }

    [Theory]
    [InlineData("123e4567-e89b-12d3-a456-426614174000")]
    [InlineData("user123")]
    [InlineData("abc-123-def")]
    public void Create_WithValidId_ShouldCreateKeycloakUserId(string validId)
    {
        // Act
        var userId = KeycloakUserId.Create(validId);

        // Assert
        userId.Value.Should().Be(validId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithInvalidId_ShouldThrowException(string invalidId)
    {
        // Act & Assert
        var action = () => KeycloakUserId.Create(invalidId);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        // Arrange
        var id1 = KeycloakUserId.Create("user123");
        var id2 = KeycloakUserId.Create("user123");

        // Act & Assert
        id1.Should().Be(id2);
    }

    [Fact]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        // Arrange
        var id1 = KeycloakUserId.Create("user123");
        var id2 = KeycloakUserId.Create("user456");

        // Act & Assert
        id1.Should().NotBe(id2);
    }

    [Fact]
    public void GetHashCode_WithSameId_ShouldReturnSameHashCode()
    {
        // Arrange
        var id1 = KeycloakUserId.Create("user123");
        var id2 = KeycloakUserId.Create("user123");

        // Act
        var hashCode1 = id1.GetHashCode();
        var hashCode2 = id2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_WithDifferentId_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var id1 = KeycloakUserId.Create("user123");
        var id2 = KeycloakUserId.Create("user456");

        // Act
        var hashCode1 = id1.GetHashCode();
        var hashCode2 = id2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        // Arrange
        const string id = "user123";
        var userId = KeycloakUserId.Create(id);

        // Act
        var result = userId.ToString();

        // Assert
        result.Should().Contain(id);
    }
}
