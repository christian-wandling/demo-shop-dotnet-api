using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoShop.Infrastructure.Common.Services;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private bool _disposed;
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_transaction != null)
        {
            throw new InvalidOperationException("The transaction is already active.");
        }

        _transaction =
            await context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_transaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_transaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing) return;

        _transaction?.Dispose();
        context.Dispose();
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
