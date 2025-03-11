#region

using DemoShop.Domain.Common.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Common.ValueObjects;

[Trait("Feature", "Common")]
public class SoftDeleteTests : Test
{
    [Fact]
    public void Create_ReturnsNewInstance_WithDefaultValues()
    {
        // Act
        var softDelete = SoftDelete.Create();

        // Assert
        softDelete.DeletedAt.Should().BeNull();
        softDelete.Deleted.Should().BeFalse();
    }

    [Fact]
    public void MarkAsDeleted_SetsDeletedAtAndDeletedFlag()
    {
        // Arrange
        var softDelete = SoftDelete.Create();
        var beforeDelete = DateTime.UtcNow;

        // Act
        Thread.Sleep(1);
        softDelete.MarkAsDeleted();

        // Assert
        softDelete.DeletedAt.Should().NotBeNull();
        softDelete.DeletedAt.Should().BeAfter(beforeDelete);
        softDelete.DeletedAt.Should().BeBefore(DateTime.UtcNow);
        softDelete.Deleted.Should().BeTrue();
    }

    [Fact]
    public void Restore_ResetsDeletedAtAndDeletedFlag()
    {
        // Arrange
        var softDelete = SoftDelete.Create();
        softDelete.MarkAsDeleted();

        // Act
        softDelete.Restore();

        // Assert
        softDelete.DeletedAt.Should().BeNull();
        softDelete.Deleted.Should().BeFalse();
    }

    [Fact]
    public void EqualityComparison_WithSameDeletedAt_ReturnsTrue()
    {
        // Arrange
        var softDelete1 = SoftDelete.Create();
        var softDelete2 = SoftDelete.Create();

        // Act
        softDelete1.MarkAsDeleted();
        softDelete2.MarkAsDeleted();

        typeof(SoftDelete)
            .GetProperty(nameof(SoftDelete.DeletedAt))!
            .SetValue(softDelete2, softDelete1.DeletedAt);

        // Assert
        softDelete1.Should().Be(softDelete2);
    }

    [Fact]
    public void EqualityComparison_WithDifferentDeletedAt_ReturnsFalse()
    {
        // Arrange
        var softDelete1 = SoftDelete.Create();
        var softDelete2 = SoftDelete.Create();

        softDelete1.MarkAsDeleted();
        Thread.Sleep(1); // Ensure different timestamp
        softDelete2.MarkAsDeleted();

        // Assert
        softDelete1.Should().NotBe(softDelete2);
    }

    [Fact]
    public void EqualityComparison_BothNotDeleted_ReturnsTrue()
    {
        // Arrange
        var softDelete1 = SoftDelete.Create();
        var softDelete2 = SoftDelete.Create();

        // Assert
        softDelete1.Should().Be(softDelete2);
    }
}
