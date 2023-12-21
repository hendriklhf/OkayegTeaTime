using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HLE.Strings;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Twitch;

public sealed class MessageRegexCreator
{
    private readonly RegexPool _regexPool = [];

    private const string PatternEnding = @"(\s|$)";
    private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase;
    private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(1);

    [SkipLocalsInit]
    public Regex Create(ReadOnlySpan<char> alias, ReadOnlySpan<char> prefix, [StringSyntax(StringSyntaxAttribute.Regex)] ReadOnlySpan<char> addition = default)
    {
        ValueStringBuilder patternBuilder = new(stackalloc char[512]);
        patternBuilder.Append('^');

        int escapedItemLength;
        if (prefix.Length == 0)
        {
            escapedItemLength = StringHelpers.RegexEscape(alias, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
            escapedItemLength = StringHelpers.RegexEscape(GlobalSettings.Suffix, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
        }
        else
        {
            escapedItemLength = StringHelpers.RegexEscape(prefix, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
            escapedItemLength = StringHelpers.RegexEscape(alias, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
        }

        patternBuilder.Append(addition, PatternEnding);
        if (_regexPool.TryGet(patternBuilder.WrittenSpan, Options, s_timeout, out Regex? cachedPattern))
        {
            return cachedPattern;
        }

        Regex compiledRegex = new(patternBuilder.ToString(), Options, s_timeout);
        _regexPool.Add(compiledRegex);
        return compiledRegex;
    }
}
