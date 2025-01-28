using Ardalis.Result;
using FluentValidation.Results;

namespace DemoShop.Application.Features.Common.Extensions;

public static class ValidationExtensions
{
    public static IEnumerable<ValidationError> ToValidationErrors(this IEnumerable<ValidationFailure> failures) =>
        failures.Select(failure => new ValidationError(failure.PropertyName, failure.ErrorMessage));
}
