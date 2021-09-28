using System.Text.RegularExpressions;
using HLE.Strings;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class StringHelper
    {
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

        public static List<string> WordArrayStringToList(this string input)
        {
            return Regex.Matches(input, @"\w+", RegexOptions.IgnoreCase).Select(m => m.Value).ToList();
        }
    }
}
