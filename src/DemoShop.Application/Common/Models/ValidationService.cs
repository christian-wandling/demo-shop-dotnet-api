#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Common.Extensions;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using FluentValidation;
using Serilog;

#endregion

namespace DemoShop.Application.Common.Models;

public class ValidationService(ILogger logger) : IValidationService
{
    public async Task<Result> ValidateAsync<TRequest>(
        TRequest request,
        IValidator<TRequest> validator,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(validator, nameof(validator));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));
        LogValidationStarted(logger, request.GetType().Name);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            LogValidationSuccess(logger, request.GetType().Name);
            return Result.Success();
        }

        var errors = validationResult.Errors.Select(e => e.ErrorMessage);
        LogValidationFailed(logger, request.GetType().Name, string.Join(", ", errors));

        return Result.Invalid(validationResult.Errors.ToValidationErrors());
    }

    private static void LogValidationStarted(ILogger logger, string request) =>
        logger.ForContext("EventId", LoggerEventId.ValidationStarted)
            .Debug("Starting to validate {Request}", request);

    private static void LogValidationSuccess(ILogger logger, string request) =>
        logger.ForContext("EventId", LoggerEventId.ValidationSuccess)
            .Information("Successfully validated {Request}", request);

    private static void LogValidationFailed(ILogger logger, string request, string errors) =>
        logger.ForContext("EventId", LoggerEventId.ValidationFailed)
            .Error("Error validating {Request}: {Errors}", request, errors);
}
