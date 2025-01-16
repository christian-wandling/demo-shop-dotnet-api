using Ardalis.Result;
using DemoShop.Domain.Common.Exceptions;
using DemoShop.Domain.Common.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DemoShop.Infrastructure.Common;

public abstract class Repository<T>(DbContext context, ILogger logger)
{
    protected DbContext Context { get; } = context;
    private ILogger Logger { get; } = logger;

    protected Result HandleDbException(string operation, Exception ex, string? identifier = null)
    {
        Logger.LogDatabaseError<T>(operation, ex, identifier);
        return Result.Error(ErrorMessages.DatabaseError<T>(operation, identifier));
    }

    protected Result<TResult> HandleDbException<TResult>(string operation, Exception ex, string? identifier = null)
        where TResult : class
    {
        var result = HandleDbException(operation, ex, identifier);
        return Result<TResult>.Error(result.Errors.FirstOrDefault() ?? string.Empty);
    }

    protected Result HandleInvalidOperationException(string operation, Exception ex, string? identifier = null)
    {
        Logger.LogDatabaseError<T>(operation, ex, identifier);
        return Result.Error(ErrorMessages.DatabaseError<T>(operation, identifier));
    }

    protected Result<TResult> HandleInvalidOperationException<TResult>(string operation, Exception ex,
        string? identifier = null)
        where TResult : class
    {
        var result = HandleInvalidOperationException(operation, ex, identifier);
        return Result<TResult>.Error(result.Errors.FirstOrDefault() ?? string.Empty);
    }

    protected Result<TResult> HandleNotFound<TResult>(string operation, string? identifier = null)
        where TResult : class
    {
        Logger.LogNotFoundError<T>(operation, identifier);
        return Result<TResult>.NotFound(ErrorMessages.NotFound<T>(operation, identifier));
    }

    protected Result HandleDeleteFailed(string operation, string? identifier = null)
    {
        Logger.LogDeleteFailedError<T>(operation, identifier);
        return Result.Error(ErrorMessages.DeleteFailed<T>(operation, identifier));
    }
}
