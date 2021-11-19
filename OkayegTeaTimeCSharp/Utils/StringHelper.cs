using System.Text.RegularExpressions;
using HLE.Strings;

namespace OkayegTeaTimeCSharp.Utils;

public static class StringHelper
{
    private static readonly Regex _onlyWhitespacesPattern = new(@"^\s+$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public static string RemoveHashtag(this string str)
    {
        return str.Remove("#");
    }

    /// <summary>
    /// Removes the <see cref="char"/> '@' from the <see cref="string"/> <paramref name="str"/>. Doesn't remove something "at" an index.
    /// </summary>
    public static string RemoveTheAt(this string str)
    {
        return str.Remove("@");
    }

    public static string Antiping(this string value)
    {
        return value.Insert(value.Length >> 1, "󠀀");
    }

    public static string NewLinesToSpaces(this string value)
    {
        return value.Remove("\r").Replace('\n', ' ');
    }

    public static bool IsNullOrEmptyOrWhitespace(this string value)
    {
        return value is null || value.Length == 0 || _onlyWhitespacesPattern.IsMatch(value);
    }
}
