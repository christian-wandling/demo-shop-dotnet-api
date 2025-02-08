#region

using DemoShop.Domain.Common.Exceptions;

#endregion

// ReSharper disable once CheckNamespace
namespace Ardalis.GuardClauses;

public static class PriceGuard
{
    /// <summary>
    ///     Throws an <see cref="ArgumentException" /> if the input string is not a valid price number.
    /// </summary>
    /// <param name="guardClause">The guard clause.</param>
    /// <param name="input">The price to validate.</param>
    /// <param name="parameterName">Name of the parameter being checked.</param>
    /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
    /// <exception cref="InvalidPriceDomainException">Thrown when input is not a valid price format.</exception>
    /// <returns>The validated price</returns>
    public static decimal InvalidPrice(
        this IGuardClause guardClause,
        decimal input,
        string? parameterName = null)
    {
        Guard.Against.NegativeOrZero(input, parameterName);

        // Check if the value exceeds decimal precision limits
        if (input >= 999999999999999.99m)
            throw new InvalidPriceDomainException($"Price '{input}' exceeds maximum allowed value");

        var decimalPlaces = (decimal.GetBits(input)[3] >> 16) & 0xFF;
        if (decimalPlaces != 0 && decimalPlaces != 2)
            throw new InvalidPriceDomainException($"Price '{input}' must have exactly 2 decimal places");

        return input;
    }
}
