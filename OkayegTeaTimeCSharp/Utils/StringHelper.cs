using HLE.Strings;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class StringHelper
    {
        public static string RemoveHashtag(this string str)
        {
            return str.Remove("#");
        }

        public static List<string> WordArrayStringToList(this string input)
        {
            return Regex.Matches(input, @"\w+", RegexOptions.IgnoreCase).Select(m => m.Value).ToList();
        }
    }
}
