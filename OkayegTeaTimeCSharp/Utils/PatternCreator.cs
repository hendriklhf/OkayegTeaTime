using OkayegTeaTimeCSharp.Commands;
using System.Text.RegularExpressions;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class PatternCreator
    {
        public static string Create(string alias, string prefix, string addition = "")
        {
            return string.IsNullOrEmpty(prefix) ? "^" + Regex.Escape(alias + CommandHelper.Suffix) + addition : "^" + Regex.Escape(prefix + alias) + addition;
        }
    }
}