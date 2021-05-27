using OkayegTeaTimeCSharp.Properties;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class StringHelper
    {
        public static string ArrayToString(this string[] input)
        {
            string result = string.Empty;
            input.ToList().ForEach(str =>
            {
                result += $"{str} ";
            });
            return result.Trim();
        }

        public static string Decode(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static byte[] Encode(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string EscapeChars(this string str)
        {
            return str.Replace("\\", "\\\\").Replace("'", "\\'");
        }

        public static bool IsMatch(this string str, string pattern)
        {
            Regex regex = new(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(str);
        }

        public static string Match(this string str, string pattern)
        {
            Regex regex = new(pattern, RegexOptions.IgnoreCase);
            return regex.Match(str).Value;
        }

        public static string ReplaceChatterinoChar(this string str)
        {
            return str.Replace(Resources.ChatterinoChar, "");
        }

        public static string ReplacePattern(this string str, string pattern, string replacement)
        {
            return Regex.Replace(str, pattern, replacement, RegexOptions.IgnoreCase);
        }

        public static string ReplaceSpaces(this string str)
        {
            return str.ReplacePattern(@"\s+", " ");
        }

        public static int ToInt(this string input)
        {
            return Convert.ToInt32(input);
        }
    }
}