using System;
using System.Text.RegularExpressions;
using HLE.Strings;

namespace OkayegTeaTime.Utils;

public static class StringHelper
{
    private static readonly Regex _channelPattern = new(@"^#?\w{3,25}$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    private const string _spanFormatDefault = "<1s";

    public static string Antiping(this string value)
    {
        return value.Insert(value.Length >> 1, HLE.Strings.StringHelper.AntipingChar);
    }

    public static string NewLinesToSpaces(this string value)
    {
        return value.ReplaceLineEndings(" ");
    }

    public static bool FormatChannel(ref string channel, bool withHashTag = false)
    {
        if (!_channelPattern.IsMatch(channel))
        {
            return false;
        }

        channel = (withHashTag switch
        {
            true => channel[0] == '#' ? channel : '#' + channel,
            _ => channel[0] == '#' ? channel[1..] : channel
        }).ToLower();

        return true;
    }

    public static int Format(this TimeSpan span, Span<char> buffer)
    {
        ValueStringBuilder builder = buffer;
        if (span.Days > 0)
        {
            builder.Append(span.Days);
            builder.Append('d');
        }

        if (span.Hours > 0)
        {
            if (builder.Length > 0)
            {
                builder.Append(", ");
            }

            builder.Append(span.Hours);
            builder.Append('h');
        }

        if (span.Minutes > 0)
        {
            if (builder.Length > 0)
            {
                builder.Append(", ");
            }

            builder.Append(span.Minutes);
            builder.Append("min");
        }

        if (span.Seconds > 0)
        {
            if (builder.Length > 0)
            {
                builder.Append(", ");
            }

            builder.Append(span.Seconds);
            builder.Append('s');
        }

        if (builder.Length == 0)
        {
            builder.Append(_spanFormatDefault);
        }

        return builder.Length;
    }

    public static string Format(this TimeSpan span)
    {
        Span<char> buffer = stackalloc char[100];
        int length = span.Format(buffer);
        return new(buffer[..length]);
    }
}
