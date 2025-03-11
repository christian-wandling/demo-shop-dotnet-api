#region

using System.Reflection;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.TestUtils.Common.Base;

#endregion

namespace DemoShop.Domain.Tests.Common.ValueObjects;

[Trait("Feature", "Common")]
public class AuditTests : Test
{
    [Fact]
    public void Create_SetsCreatedAtAndModifiedAt_ToCurrentTime()
    {
        // Arrange
        var beforeCreate = DateTime.UtcNow;

        // Act
        var audit = Audit.Create();
        var afterCreate = DateTime.UtcNow;

        // Assert
        audit.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        audit.CreatedAt.Should().BeOnOrBefore(afterCreate);
        audit.ModifiedAt.Should().Be(audit.CreatedAt);
    }

    [Fact]
    public void UpdateModified_UpdatesModifiedAtOnly()
    {
        // Arrange
        var audit = Audit.Create();
        var originalCreatedAt = audit.CreatedAt;
        var beforeUpdate = DateTime.UtcNow;

        // Act
        audit.UpdateModified();
        var afterUpdate = DateTime.UtcNow;

        // Assert
        audit.CreatedAt.Should().Be(originalCreatedAt);
        audit.ModifiedAt.Should().BeOnOrAfter(beforeUpdate);
        audit.ModifiedAt.Should().BeOnOrBefore(afterUpdate);
        audit.ModifiedAt.Should().BeAfter(audit.CreatedAt);
    }

    [Fact]
    public void EqualityComparison_WithSameTimestamps_ReturnsTrue()
    {
        // Arrange
        var audit1 = Audit.Create();
        var audit2 = Audit.Create();

        // Use reflection to set the same timestamps
        var timestamps = new { CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow };

        var backingField = typeof(Audit)
            .GetField("<CreatedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(audit1, timestamps.CreatedAt);
        backingField?.SetValue(audit2, timestamps.CreatedAt);
        typeof(Audit)
            .GetProperty(nameof(Audit.ModifiedAt))!
            .SetValue(audit1, timestamps.ModifiedAt);
        typeof(Audit)
            .GetProperty(nameof(Audit.ModifiedAt))!
            .SetValue(audit2, timestamps.ModifiedAt);

        // Assert
        audit1.Should().Be(audit2);
    }

    [Fact]
    public void EqualityComparison_WithDifferentTimestamps_ReturnsFalse()
    {
        // Arrange
        var audit1 = Audit.Create();
        Thread.Sleep(1); // Ensure different timestamp
        var audit2 = Audit.Create();

        // Assert
        audit1.Should().NotBe(audit2);
    }

    [Fact]
    public void MultipleModifications_UpdatesModifiedAtEachTime()
    {
        // Arrange
        var audit = Audit.Create();
        var firstModified = audit.ModifiedAt;

        // Act
        Thread.Sleep(1); // Ensure different timestamp
        audit.UpdateModified();
        var secondModified = audit.ModifiedAt;

        Thread.Sleep(1); // Ensure different timestamp
        audit.UpdateModified();
        var thirdModified = audit.ModifiedAt;

        // Assert
        secondModified.Should().BeAfter(firstModified);
        thirdModified.Should().BeAfter(secondModified);
    }
}
