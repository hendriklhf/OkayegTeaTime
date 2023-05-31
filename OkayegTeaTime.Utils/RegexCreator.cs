using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using HLE.Strings;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Utils;

public sealed class RegexCreator
{
    private readonly RegexPool _cachedPatterns = new();

    private const string _patternEnding = @"(\s|$)";
    private const RegexOptions _regexOptions = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase;

    public Regex Create(ReadOnlySpan<char> alias, ReadOnlySpan<char> prefix, [StringSyntax(StringSyntaxAttribute.Regex)] ReadOnlySpan<char> addition = default)
    {
        ValueStringBuilder patternBuilder = stackalloc char[512];
        patternBuilder.Append('^');

        int escapedItemLength;
        if (prefix.Length == 0)
        {
            escapedItemLength = HLE.Strings.StringHelper.RegexEscape(alias, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
            escapedItemLength = HLE.Strings.StringHelper.RegexEscape(AppSettings.Suffix, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
        }
        else
        {
            escapedItemLength = HLE.Strings.StringHelper.RegexEscape(prefix, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
            escapedItemLength = HLE.Strings.StringHelper.RegexEscape(alias, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
        }

        patternBuilder.Append(addition, _patternEnding);
        if (_cachedPatterns.TryGet(patternBuilder.WrittenSpan, _regexOptions, TimeSpan.FromSeconds(1), out Regex? cachedPattern))
        {
            return cachedPattern;
        }

        Regex compiledRegex = new(patternBuilder.ToString(), _regexOptions, TimeSpan.FromSeconds(1));
        _cachedPatterns.Add(compiledRegex);
        return compiledRegex;
    }
}
