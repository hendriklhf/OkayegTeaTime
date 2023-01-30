using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Utils;

public sealed class RegexCreator
{
    private readonly IDictionary<string, Regex> _cachedPatterns = new ConcurrentDictionary<string, Regex>();
    private readonly ArrayPool<string?> _stringArrayPool = ArrayPool<string?>.Create();

    private const string _patternEnding = @"(\s|$)";

    public Regex Create(string alias, string? prefix, [StringSyntax(StringSyntaxAttribute.Regex)] string? addition = null)
    {
        string?[] rentedArray = _stringArrayPool.Rent(3);
        try
        {
            Span<string?> patternItems = rentedArray;
            patternItems[0] = prefix;
            patternItems[1] = alias;
            patternItems[2] = AppSettings.Suffix;

            bool isEmpty = string.IsNullOrEmpty(prefix);
            byte isEmptyAsByte = Unsafe.As<bool, byte>(ref isEmpty);
            StringBuilder builder = stackalloc char[512];
            builder.Append('^');

            Span<char> escapedItem = stackalloc char[100];
            int length = StringHelper.RegexEscape(patternItems[isEmptyAsByte], escapedItem);
            builder.Append(escapedItem[..length]);
            length = StringHelper.RegexEscape(patternItems[++isEmptyAsByte], escapedItem);
            builder.Append(escapedItem[..length]);

            builder.Append(addition, _patternEnding);
            string patternKey = builder.ToString();

            if (_cachedPatterns.TryGetValue(patternKey, out Regex? cachedPattern))
            {
                return cachedPattern;
            }

            Regex compiledRegex = new(patternKey, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
            _cachedPatterns.Add(patternKey, compiledRegex);
            return compiledRegex;
        }
        finally
        {
            _stringArrayPool.Return(rentedArray);
        }
    }
}
