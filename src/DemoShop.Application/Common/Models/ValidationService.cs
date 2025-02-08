#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Common.Extensions;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using FluentValidation;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Common.Models;

public class ValidationService(ILogger<ValidationService> logger) : IValidationService
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

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
            return Result.Success();

        var errors = validationResult.Errors.Select(e => e.ErrorMessage);

        logger.LogValidationFailed(
            $"Validation failed for {typeof(TRequest).Name}",
            string.Join(", ", errors));

        return Result.Invalid(validationResult.Errors.ToValidationErrors());
    }
}
