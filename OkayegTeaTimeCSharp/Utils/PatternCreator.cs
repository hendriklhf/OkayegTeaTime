using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace OkayegTeaTimeCSharp.Utils;

public static class PatternCreator
{
    private static readonly IDictionary<string, Regex> _cachedPatterns = new ConcurrentDictionary<string, Regex>();

    public static Regex Create(string alias, string prefix, string addition = "")
    {
        // TODO: kinda don't like how this is passed in
        var patternPrefix = string.IsNullOrEmpty(prefix)
            ? "^" + Regex.Escape(alias + Config.Suffix) + addition
            : "^" + Regex.Escape(prefix + alias) + addition;

        var patternKey = patternPrefix + addition;

        if (_cachedPatterns.TryGetValue(patternKey, out Regex cachedPattern))
        {
            return cachedPattern;
        }

        var compiledRegex = new Regex(patternKey, RegexOptions.Compiled | RegexOptions.Singleline);
        _cachedPatterns.Add(patternKey, compiledRegex);

        return compiledRegex;
    }
}
