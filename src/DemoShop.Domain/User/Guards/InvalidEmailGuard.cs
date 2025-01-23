using System.Runtime.CompilerServices;
using DemoShop.Domain.User.Exceptions;

// ReSharper disable once CheckNamespace
namespace Ardalis.GuardClauses
{
    public static class InvalidEmailGuard
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input string is not a valid email address.
        /// </summary>
        /// <param name="guardClause">The guard clause.</param>
        /// <param name="input">The email address to validate.</param>
        /// <param name="parameterName">Name of the parameter being checked.</param>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="InvalidEmailDomainException">Thrown when input is not a valid email format.</exception>
        /// <returns>The validated email address</returns>
        public static string InvalidEmail(
            this IGuardClause guardClause,
            string input,
            [CallerArgumentExpression(nameof(input))] string? parameterName = null)
        {
            Guard.Against.NullOrWhiteSpace(input, parameterName);

            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(input);
                if (input != mailAddress.Address)
                {
                    throw new InvalidEmailDomainException($"Email '{input}' is not in a valid format");
                }
            }
            catch (FormatException)
            {
                throw new InvalidEmailDomainException($"Email '{input}' is not in a valid format");
            }

            return input;
        }
    }
}
