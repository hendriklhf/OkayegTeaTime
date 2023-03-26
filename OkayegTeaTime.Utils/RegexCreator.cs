using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Utils;

public sealed class RegexCreator
{
    private readonly IDictionary<int, Regex> _cachedPatterns = new ConcurrentDictionary<int, Regex>();

    private const string _patternEnding = @"(\s|$)";

    public Regex Create(ReadOnlySpan<char> alias, ReadOnlySpan<char> prefix, [StringSyntax(StringSyntaxAttribute.Regex)] ReadOnlySpan<char> addition = default)
    {
        StringBuilder patternBuilder = stackalloc char[512];
        patternBuilder.Append('^');

        int escapedItemLength;
        if (prefix.Length == 0)
        {
            escapedItemLength = HLE.StringHelper.RegexEscape(alias, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
            escapedItemLength = HLE.StringHelper.RegexEscape(AppSettings.Suffix, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
        }
        else
        {
            escapedItemLength = HLE.StringHelper.RegexEscape(prefix, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
            escapedItemLength = HLE.StringHelper.RegexEscape(alias, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
        }

        patternBuilder.Append(addition, _patternEnding);
        int patternHashCode = string.GetHashCode(patternBuilder.WrittenSpan);
        if (_cachedPatterns.TryGetValue(patternHashCode, out Regex? cachedPattern))
        {
            return cachedPattern;
        }

        Regex compiledRegex = new(patternBuilder.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
        _cachedPatterns.Add(patternHashCode, compiledRegex);
        return compiledRegex;
    }
}
