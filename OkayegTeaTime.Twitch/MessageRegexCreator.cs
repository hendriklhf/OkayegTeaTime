using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HLE.Strings;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Twitch;

public sealed class MessageRegexCreator : IDisposable
{
    private readonly RegexPool _regexPool = new();

    private const string _patternEnding = @"(\s|$)";
    private const RegexOptions _regexOptions = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase;

    [SkipLocalsInit]
    public Regex Create(ReadOnlySpan<char> alias, ReadOnlySpan<char> prefix, [StringSyntax(StringSyntaxAttribute.Regex)] ReadOnlySpan<char> addition = default)
    {
        ValueStringBuilder patternBuilder = new(stackalloc char[512]);
        patternBuilder.Append('^');

        int escapedItemLength;
        if (prefix.Length == 0)
        {
            escapedItemLength = StringHelper.RegexEscape(alias, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
            escapedItemLength = StringHelper.RegexEscape(AppSettings.Suffix, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
        }
        else
        {
            escapedItemLength = StringHelper.RegexEscape(prefix, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
            escapedItemLength = StringHelper.RegexEscape(alias, patternBuilder.FreeBuffer);
            patternBuilder.Advance(escapedItemLength);
        }

        patternBuilder.Append(addition, _patternEnding);
        if (_regexPool.TryGet(patternBuilder.WrittenSpan, _regexOptions, TimeSpan.FromSeconds(1), out Regex? cachedPattern))
        {
            return cachedPattern;
        }

        Regex compiledRegex = new(patternBuilder.ToString(), _regexOptions, TimeSpan.FromSeconds(1));
        _regexPool.Add(compiledRegex);
        return compiledRegex;
    }

    public void Dispose()
    {
        _regexPool.Dispose();
    }
}
