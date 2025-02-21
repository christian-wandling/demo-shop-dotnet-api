#region

using DemoShop.Application.Common.Interfaces;
using DemoShop.Infrastructure.Common.Services;
using DemoShop.TestUtils.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

#endregion

namespace DemoShop.Infrastructure.Tests.Common.Services;

[Trait("Category", "Unit")]
[Trait("Layer", "Infrastructure")]
[Trait("Feature", "Common")]
public class UnitOfWorkTests : Test
{
    private readonly IApplicationDbContext _context;
    private readonly UnitOfWork _sut;
    private readonly IDbContextTransaction _transaction;

    public UnitOfWorkTests()
    {
        _context = Substitute.For<IApplicationDbContext>();
        _transaction = Substitute.For<IDbContextTransaction>();
        var database = Substitute.For<DatabaseFacade>(Substitute.For<DbContext>());

        _context.Database.Returns(database);
        database.BeginTransactionAsync(Arg.Any<CancellationToken>())
            .Returns(_transaction);

        _sut = new UnitOfWork(_context);
    }

    [Fact]
    public void HasActiveTransaction_WhenNoTransaction_ReturnsFalse()
    {
        // Act
        var result = _sut.HasActiveTransaction;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task BeginTransactionAsync_WhenCalled_StartsNewTransaction()
    {
        // Act
        await _sut.BeginTransactionAsync(CancellationToken.None);

        // Assert
        await _context.Database.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        _sut.HasActiveTransaction.Should().BeTrue();
    }

    [Fact]
    public async Task BeginTransactionAsync_WhenTransactionActive_ThrowsInvalidOperationException()
    {
        // Arrange
        await _sut.BeginTransactionAsync(CancellationToken.None);

        // Act
        var act = () => _sut.BeginTransactionAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("The transaction is already active.");
    }

    [Fact]
    public async Task CommitTransactionAsync_WhenTransactionActive_CommitsAndDisposesTransaction()
    {
        // Arrange
        await _sut.BeginTransactionAsync(CancellationToken.None);

        // Act
        await _sut.CommitTransactionAsync(CancellationToken.None);

        // Assert
        await _transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        await _transaction.Received(1).DisposeAsync();
        _sut.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task CommitTransactionAsync_WhenNoActiveTransaction_ThrowsInvalidOperationException()
    {
        // Act
        var act = () => _sut.CommitTransactionAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No active transaction to commit.");
    }

    [Fact]
    public async Task RollbackTransactionAsync_WhenTransactionActive_RollsBackAndDisposesTransaction()
    {
        // Arrange
        await _sut.BeginTransactionAsync(CancellationToken.None);

        // Act
        await _sut.RollbackTransactionAsync(CancellationToken.None);

        // Assert
        await _transaction.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _transaction.Received(1).DisposeAsync();
        _sut.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public void Dispose_WhenCalled_DisposesContextAndTransaction()
    {
        // Act
        _sut.Dispose();

        // Assert
        _context.Received(1).Dispose();
    }

    [Fact]
    public async Task Dispose_WhenCalledWithActiveTransaction_DisposesTransactionAndContext()
    {
        // Arrange
        await _sut.BeginTransactionAsync(CancellationToken.None);

        // Act
        _sut.Dispose();

        // Assert
        _transaction.Received(1).Dispose();
        _context.Received(1).Dispose();
        _sut.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public void DisposedObject_WhenCallingMethods_ThrowsObjectDisposedException()
    {
        // Arrange
        _sut.Dispose();

        // Act & Assert
        var beginTransaction = () => _sut.BeginTransactionAsync(CancellationToken.None);
        var commitTransaction = () => _sut.CommitTransactionAsync(CancellationToken.None);
        var rollbackTransaction = () => _sut.RollbackTransactionAsync(CancellationToken.None);

        beginTransaction.Should().ThrowAsync<ObjectDisposedException>();
        commitTransaction.Should().ThrowAsync<ObjectDisposedException>();
        rollbackTransaction.Should().ThrowAsync<ObjectDisposedException>();
    }
}
