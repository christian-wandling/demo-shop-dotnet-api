#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Logging;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

#endregion

namespace DemoShop.Api.Common.Middleware;

public class GlobalExceptionHandler(ILogger logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(exception, nameof(exception));
        Guard.Against.Null(httpContext, nameof(httpContext));

        var problemDetails = exception switch
        {
            DomainException ex => CreateProblemDetails(
                StatusCodes.Status422UnprocessableEntity,
                "Domain Error",
                ex.Message,
                () => LogUnhandledDomainException(logger, ex.Message)
            ),
            ValidationException ex => CreateProblemDetails(
                StatusCodes.Status422UnprocessableEntity,
                "Validation Error",
                ex.Message,
                () => LogUnhandledValidationException(logger, ex.Message)
            ),
            UnauthorizedAccessException ex => CreateProblemDetails(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                ex.Message,
                () => LogUnhandledAuthException(logger, ex.Message)
            ),
            InvalidOperationException ex => CreateProblemDetails(
                StatusCodes.Status409Conflict,
                "Invalid Operation",
                ex.Message,
                () => LogUnhandledDbException(logger, ex.Message)
            ),
            ArgumentException ex => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Bad Request",
                ex.Message,
                () => LogUnhandledDomainException(logger, ex.Message)
            ),
            { } ex => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Bad Request",
                ex.Message,
                () => LogUnhandledException(logger, ex.Message)
            ),
            _ => null
        };

        if (problemDetails is null)
            return true;

        if (problemDetails.Status != null)
            httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static ProblemDetails CreateProblemDetails(
        int status,
        string title,
        string detail,
        Action logAction)
    {
        logAction();
        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://tools.ietf.org/html/rfc7231#section-6.5.{status - 399}"
        };
    }

    private static void LogUnhandledDomainException(ILogger logger, string message) =>
        logger.Error("[{EventId}] {Message}", LoggerEventIds.UnhandledDomainException, message);

    private static void LogUnhandledValidationException(ILogger logger, string message) =>
        logger.Error("[{EventId}] {Message}", LoggerEventIds.UnhandledValidationException, message);

    private static void LogUnhandledAuthException(ILogger logger, string message) =>
        logger.Error("[{EventId}] {Message}", LoggerEventIds.UnhandledAuthException, message);

    private static void LogUnhandledDbException(ILogger logger, string message) =>
        logger.Error("[{EventId}] {Message}", LoggerEventIds.UnhandledDbException, message);

    private static void LogUnhandledException(ILogger logger, string message) =>
        logger.Error("[{EventId}] {Message}", LoggerEventIds.UnhandledException, message);
}
