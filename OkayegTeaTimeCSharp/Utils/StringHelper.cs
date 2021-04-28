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

        public static string Match(this string input, string pattern)
        {
            Regex regex = new(pattern);
            return regex.Match(input).Value;
        }

        public static bool IsMatch(this string input, string pattern)
        {
            Regex regex = new(pattern);
            return regex.IsMatch(input);
        }

        public static string ReplacePattern(this string input, string pattern, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        public static string ReplaceSpaces(this string input)
        {
            return input.ReplacePattern(@"\s+", " ");
        }
    }
}
