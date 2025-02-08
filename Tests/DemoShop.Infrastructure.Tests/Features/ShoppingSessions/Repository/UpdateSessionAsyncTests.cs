#region

using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Infrastructure.Features.ShoppingSessions;
using DemoShop.Infrastructure.Tests.Common.Base;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Infrastructure.Tests.Features.ShoppingSessions.Repository;

[Trait("Feature", "ShoppingSession")]
public class UpdateSessionAsyncTests : RepositoryTest
{
    private readonly ShoppingSessionRepository _sut;

    public UpdateSessionAsyncTests(ITestOutputHelper output) : base(output)
    {
        _sut = new ShoppingSessionRepository(Context);
    }

    [Fact]
    public async Task ShouldAddCartItem_WhenSessionExists()
    {
        // Arrange
        var unsavedSession = Create<ShoppingSessionEntity>();
        var savedSession = await AddTestDataAsync(unsavedSession);
        var productId = Create<int>();
        savedSession.AddCartItem(productId);

        // Act
        var result = await _sut.UpdateSessionAsync(savedSession, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CartItems.Should().HaveCount(1);
        result.CartItems.Should().Contain(item => item.ProductId == productId);
    }

    [Fact]
    public async Task ShouldRemoveCartItem_WhenSessionExists()
    {
        // Arrange
        var unsavedSession = Create<ShoppingSessionEntity>();
        var savedSession = await AddTestDataAsync(unsavedSession);
        var productId = Create<int>();
        savedSession.AddCartItem(productId);
        var savedSessionWithItem = await UpdateTestDataAsync(unsavedSession);
        var itemId = savedSessionWithItem.CartItems.First().Id;
        savedSessionWithItem.RemoveCartItem(itemId);

        // Act
        var result = await _sut.UpdateSessionAsync(savedSession, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CartItems.Should().HaveCount(0);
        result.CartItems.Should().NotContain(item => item.Id == itemId);
    }

    [Fact]
    public async Task ShouldUpdateCartItemQuantity_WhenSessionExists()
    {
        // Arrange
        var unsavedSession = Create<ShoppingSessionEntity>();
        var savedSession = await AddTestDataAsync(unsavedSession);
        var productId = Create<int>();
        savedSession.AddCartItem(productId);
        var savedSessionWithItem = await UpdateTestDataAsync(unsavedSession);
        var itemId = savedSessionWithItem.CartItems.First().Id;
        var quantity = Create<int>();
        savedSessionWithItem.UpdateCartItem(itemId, quantity);

        // Act
        var result = await _sut.UpdateSessionAsync(savedSessionWithItem, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CartItems.Should().HaveCount(1);
        result.CartItems.Should().Contain(item => item.ProductId == productId);
        result.CartItems.First(c => c.Id == itemId).Quantity.Value.Should().Be(quantity);
    }

    [Fact]
    public async Task ShouldThrow_WhenShoppingSessionEntityNull()
    {
        // Arrange
        ShoppingSessionEntity? nullSession = null;

        // Act
        var act = () => _sut.CreateSessionAsync(nullSession!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
