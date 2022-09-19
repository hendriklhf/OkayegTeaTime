using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Utils;

public static class PatternCreator
{
    public static int CacheSize => _cachedPatterns.Count;

    private static readonly IDictionary<string, Regex> _cachedPatterns = new ConcurrentDictionary<string, Regex>();

    public static Regex Create(string alias, string? prefix, string? addition = null)
    {
        // TODO: kinda don't like how this is passed in
        string patternPrefix = '^' + Regex.Escape(string.IsNullOrEmpty(prefix) ? alias + AppSettings.Suffix : prefix + alias);
        addition ??= string.Empty;
        string patternKey = patternPrefix + addition + @"(\s|$)";

        if (_cachedPatterns.TryGetValue(patternKey, out Regex? cachedPattern))
        {
            return cachedPattern;
        }

        Regex compiledRegex = new(patternKey, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
        _cachedPatterns.Add(patternKey, compiledRegex);
        return compiledRegex;
    }
}
