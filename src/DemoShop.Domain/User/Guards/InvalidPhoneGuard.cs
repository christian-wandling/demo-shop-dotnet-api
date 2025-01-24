using System.Text.RegularExpressions;
using DemoShop.Domain.Common.Constants;
using DemoShop.Domain.User.Exceptions;

// ReSharper disable once CheckNamespace
namespace Ardalis.GuardClauses
{
    public static partial class PhoneGuard
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input string is not a valid phone number.
        /// </summary>
        /// <param name="guardClause">The guard clause.</param>
        /// <param name="input">The phone number to validate.</param>
        /// <param name="parameterName">Name of the parameter being checked.</param>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="InvalidPhoneDomainException">Thrown when input is  not a valid phone format.</exception>
        /// <returns>The validated phone number</returns>
        public static string? InvalidPhone(
            this IGuardClause guardClause,
            string? input,
            string? parameterName = null)
        {
            if (input is not null && !ValidationConstants.PhoneNumber().IsMatch(input))
            {
                throw new InvalidPhoneDomainException($"Phone number '{input}' is not in a valid format");
            }

            return input;
        }
 }
}
