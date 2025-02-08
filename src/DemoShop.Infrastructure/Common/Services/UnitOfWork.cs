#region

using DemoShop.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

#endregion

namespace DemoShop.Infrastructure.Common.Services;

public sealed class UnitOfWork(IApplicationDbContext context) : IUnitOfWork
{
    private bool _disposed;
    private IDbContextTransaction? _transaction;

    public bool HasActiveTransaction => _transaction != null;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (HasActiveTransaction) throw new InvalidOperationException("The transaction is already active.");

        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!HasActiveTransaction) throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            await _transaction!.CommitAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!HasActiveTransaction) return;

        try
        {
            await _transaction!.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private async Task DisposeTransactionAsync()
    {
        if (HasActiveTransaction)
        {
            await _transaction!.DisposeAsync();
            _transaction = null;
        }
    }

    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing) return;

        if (HasActiveTransaction)
        {
            _transaction?.Dispose();
            _transaction = null;
        }

        context.Dispose();
        _disposed = true;
    }
}
