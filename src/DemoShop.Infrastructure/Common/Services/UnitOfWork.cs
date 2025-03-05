#region

using DemoShop.Application.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Common.Services;

public sealed class UnitOfWork(IApplicationDbContext context, ILogger logger) : IUnitOfWork
{
    private bool _disposed;
    private IDbContextTransaction? _transaction;

    public bool HasActiveTransaction => _transaction != null;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (HasActiveTransaction) throw new InvalidOperationException("The transaction is already active.");

        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        LogTransactionStarted(logger);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!HasActiveTransaction) throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            await _transaction!.CommitAsync(cancellationToken);
            LogTransactionSucceeded(logger);
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
            LogTransactionRollback(logger);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public void Dispose()
    {
        if (HasActiveTransaction)
        {
            _transaction?.Dispose();
            _transaction = null;
        }

        context.Dispose();
        _disposed = true;
        LogTransactionDisposed(logger);
    }

    private async Task DisposeTransactionAsync()
    {
        if (HasActiveTransaction)
        {
            await _transaction!.DisposeAsync();
            _transaction = null;
            LogTransactionDisposed(logger);
        }
    }

    private static void LogTransactionStarted(ILogger logger) =>
        logger.ForContext("EventId", LoggerEventIds.TransactionStarted)
            .Information("Starting transaction");

    private static void LogTransactionSucceeded(ILogger logger) =>
        logger.ForContext("EventId", LoggerEventIds.TransactionSuccess)
            .Information("Transaction completed successfully");

    private static void LogTransactionRollback(ILogger logger) =>
        logger.ForContext("EventId", LoggerEventIds.TransactionRollback)
            .Error("Transaction rolled back");

    private static void LogTransactionDisposed(ILogger logger) =>
        logger.ForContext("EventId", LoggerEventIds.TransactionDisposed)
            .Debug("Transaction disposed");
}
