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
        StringBuilder builder = stackalloc char[512];
        builder.Append('^');

        Span<char> escapedItem = stackalloc char[100];
        int escapedItemLength;
        if (prefix.Length == 0)
        {
            escapedItemLength = HLE.StringHelper.RegexEscape(alias, escapedItem);
            builder.Append(escapedItem[..escapedItemLength]);
            escapedItemLength = HLE.StringHelper.RegexEscape(AppSettings.Suffix, escapedItem);
            builder.Append(escapedItem[..escapedItemLength]);
        }
        else
        {
            escapedItemLength = HLE.StringHelper.RegexEscape(prefix, escapedItem);
            builder.Append(escapedItem[..escapedItemLength]);
            escapedItemLength = HLE.StringHelper.RegexEscape(alias, escapedItem);
            builder.Append(escapedItem[..escapedItemLength]);
        }

        builder.Append(addition, _patternEnding);
        int patternHashCode = string.GetHashCode(builder.WrittenSpan);
        if (_cachedPatterns.TryGetValue(patternHashCode, out Regex? cachedPattern))
        {
            return cachedPattern;
        }

        Regex compiledRegex = new(builder.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
        _cachedPatterns.Add(patternHashCode, compiledRegex);
        return compiledRegex;
    }
}
