#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Logging;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DemoShop.Api.Common.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
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
                () => logger.LogDomainException(ex.Message)
            ),
            ValidationException ex => CreateProblemDetails(
                StatusCodes.Status422UnprocessableEntity,
                "Validation Error",
                ex.Message,
                () => logger.LogValidationFailed("", ex.Message)
            ),
            UnauthorizedAccessException ex => CreateProblemDetails(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                ex.Message,
                () => logger.LogAuthFailed(ex.Message)
            ),
            InvalidOperationException ex => CreateProblemDetails(
                StatusCodes.Status409Conflict,
                "Invalid Operation",
                ex.Message,
                () => logger.LogOperationFailed("Operation failed", "", "", ex)
            ),
            ArgumentException ex => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Bad Request",
                ex.Message,
                () => logger.LogOperationFailed("Operation failed", "", "", ex)
            ),
            _ => null
        };

        if (problemDetails is null) return true;

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
}
