using OkayegTeaTimeCSharp.Properties;
using System.Text;
using System.Text.RegularExpressions;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class StringHelper
    {
        public static byte[] ToByteArray(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string ToString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string EscapeChars(this string str)
        {
            return str.Replace("\\", "\\\\").Replace("'", "\\'");
        }

        public static string ReplaceChatterinoChar(this string str)
        {
            return str.Replace(Resources.ChatterinoChar, "");
        }

        public static string Match(this string input, string pattern)
        {
            Regex regex = new(pattern, RegexOptions.IgnoreCase);
            return regex.Match(input).Value;
        }

        public static bool IsMatch(this string input, string pattern)
        {
            Regex regex = new(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(input);
        }

        public static string ReplacePattern(this string input, string pattern, string replacement)
        {
            return Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
        }

        public static string ReplaceSpaces(this string input)
        {
            return input.ReplacePattern(@"\s+", " ");
        }
    }
}
