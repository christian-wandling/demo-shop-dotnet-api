#region

using Ardalis.Result;
using FluentValidation.Results;

#endregion

namespace DemoShop.Application.Common.Extensions;

public static class ValidationExtensions
{
    public static IEnumerable<ValidationError> ToValidationErrors(this IEnumerable<ValidationFailure> failures) =>
        failures.Select(failure => new ValidationError(failure.PropertyName, failure.ErrorMessage));
}
