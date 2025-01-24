using System.Text.RegularExpressions;

namespace DemoShop.Domain.Common.Constants;

public static partial class ValidationConstants
{
    [GeneratedRegex(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled)]
    public static partial Regex PhoneNumber();
}
