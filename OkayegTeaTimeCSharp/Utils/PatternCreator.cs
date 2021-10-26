using System.Text.RegularExpressions;
using OkayegTeaTimeCSharp.Twitch;

namespace OkayegTeaTimeCSharp.Utils;

public static class PatternCreator
{
    public static string Create(string alias, string prefix, string addition = "")
    {
        return string.IsNullOrEmpty(prefix) ? "^" + Regex.Escape(alias + TwitchConfig.Suffix) + addition : "^" + Regex.Escape(prefix) + Regex.Escape(alias) + addition;
    }
}
