using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Utils;

public static class StringHelper
{
    private static readonly Regex _channelPattern = new(@"^#?\w{3,25}$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    private const string _spanFormatDefault = "<1s";

    public static string Antiping(this string value)
    {
        return value.Insert(value.Length >> 1, HLE.StringHelper.AntipingChar);
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
        int resultLength = 0;
        if (span.Days > 0)
        {
            string days = span.Days.ToString(CultureInfo.InvariantCulture);
            days.CopyTo(buffer[resultLength..]);
            resultLength += days.Length;
            buffer[resultLength++] = 'd';
        }

        if (span.Hours > 0)
        {
            if (resultLength > 0)
            {
                ", ".CopyTo(buffer[resultLength..]);
                resultLength += ", ".Length;
            }

            string hours = span.Hours.ToString(CultureInfo.InvariantCulture);
            hours.CopyTo(buffer[resultLength..]);
            resultLength += hours.Length;
            buffer[resultLength++] = 'h';
        }

        if (span.Minutes > 0)
        {
            if (resultLength > 0)
            {
                ", ".CopyTo(buffer[resultLength..]);
                resultLength += ", ".Length;
            }

            string minutes = span.Minutes.ToString(CultureInfo.InvariantCulture);
            minutes.CopyTo(buffer[resultLength..]);
            resultLength += minutes.Length;
            buffer[resultLength++] = 'm';
            buffer[resultLength++] = 'i';
            buffer[resultLength++] = 'n';
        }

        if (span.Seconds > 0)
        {
            if (resultLength > 0)
            {
                ", ".CopyTo(buffer[resultLength..]);
                resultLength += ", ".Length;
            }

            string seconds = span.Seconds.ToString(CultureInfo.InvariantCulture);
            seconds.CopyTo(buffer[resultLength..]);
            resultLength += seconds.Length;
            buffer[resultLength++] = 's';
        }

        if (resultLength != 0)
        {
            return resultLength;
        }

        _spanFormatDefault.CopyTo(buffer);
        resultLength += _spanFormatDefault.Length;
        return resultLength;
    }

    public static string Format(this TimeSpan span)
    {
        Span<char> buffer = stackalloc char[100];
        int length = span.Format(buffer);
        return new(buffer[..length]);
    }
}
