using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class StringHelper
    {
        public static string RemoveHashtag(this string str)
        {
            return str.Replace("#", "");
        }

        public static List<string> WordArrayStringToList(this string input)
        {
            List<string> result = new();
            Regex.Matches(input, @"\w+", RegexOptions.IgnoreCase).ToList().ForEach(m =>
            {
                result.Add(m.Value);
            });
            return result;
        }
    }
}
