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
    private readonly IDictionary<string, Regex> _cachedPatterns = new ConcurrentDictionary<string, Regex>();

    private const string _patternEnding = @"(\s|$)";

    public Regex Create(string alias, string? prefix, [StringSyntax(StringSyntaxAttribute.Regex)] string? addition = null)
    {
        StringBuilder builder = stackalloc char[512];
        builder.Append('^');

        Span<char> escapedItem = stackalloc char[100];
        int escapedItemLength;
        if (string.IsNullOrEmpty(prefix))
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
        string pattern = builder.ToString();

        if (_cachedPatterns.TryGetValue(pattern, out Regex? cachedPattern))
        {
            return cachedPattern;
        }

        Regex compiledRegex = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
        _cachedPatterns.Add(pattern, compiledRegex);
        return compiledRegex;
    }
}
