#region

using System.Text.RegularExpressions;

#endregion

namespace DemoShop.Domain.Common.Constants;

public static class ValidationConstants
{
    public static readonly Regex PhoneNumber = new(@"^(\+\d{1,3}[-\s]?)?\d{1,14}$", RegexOptions.Compiled);
}
