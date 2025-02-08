#region

using Ardalis.Result;
using FluentValidation;

#endregion

namespace DemoShop.Application.Common.Interfaces;

public interface IValidationService
{
    Task<Result> ValidateAsync<TRequest>(
        TRequest request,
        IValidator<TRequest> validator,
        CancellationToken cancellationToken);
}
