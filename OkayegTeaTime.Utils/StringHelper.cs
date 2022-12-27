using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Utils;

public static class StringHelper
{
    private static readonly Regex _channelPattern = new(@"^#?\w{3,25}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    private const string _commaSpace = ", ";
    private const string _spanFormatDefault = "<1s";

    public static string Antiping(this string value)
    {
        return value.Insert(value.Length >> 1, "󠀀");
    }

    public static string NewLinesToSpaces(this string value)
    {
        return value.ReplaceLineEndings(HLE.StringHelper.Whitespace);
    }

    public static bool FormatChannel(ref string channel, bool withHashTag = false)
    {
        if (!_channelPattern.IsMatch(channel))
        {
            return false;
        }

        HLE.StringHelper.ToLower(channel);
        channel = withHashTag switch
        {
            true => channel[0] == '#' ? channel : '#' + channel,
            _ => channel[0] == '#' ? channel[1..] : channel
        };

        return true;
    }

    public static string Format(this TimeSpan span)
    {
        Span<char> resultBuffer = stackalloc char[100];
        int resultLength = 0;
        if (span.Days > 0)
        {
            string days = span.Days.ToString(CultureInfo.InvariantCulture);
            days.CopyTo(resultBuffer[resultLength..]);
            resultLength += days.Length;
            resultBuffer[resultLength++] = 'd';
        }

        if (span.Hours > 0)
        {
            if (resultLength > 0)
            {
                _commaSpace.CopyTo(resultBuffer[resultLength..]);
                resultLength += _commaSpace.Length;
            }

            string hours = span.Hours.ToString(CultureInfo.InvariantCulture);
            hours.CopyTo(resultBuffer[resultLength..]);
            resultLength += hours.Length;
            resultBuffer[resultLength++] = 'h';
        }

        if (span.Minutes > 0)
        {
            if (resultLength > 0)
            {
                _commaSpace.CopyTo(resultBuffer[resultLength..]);
                resultLength += _commaSpace.Length;
            }

            string minutes = span.Minutes.ToString(CultureInfo.InvariantCulture);
            minutes.CopyTo(resultBuffer[resultLength..]);
            resultLength += minutes.Length;
            resultBuffer[resultLength++] = 'm';
            resultBuffer[resultLength++] = 'i';
            resultBuffer[resultLength++] = 'n';
        }

        if (span.Seconds > 0)
        {
            if (resultLength > 0)
            {
                _commaSpace.CopyTo(resultBuffer[resultLength..]);
                resultLength += _commaSpace.Length;
            }

            string seconds = span.Seconds.ToString(CultureInfo.InvariantCulture);
            seconds.CopyTo(resultBuffer[resultLength..]);
            resultLength += seconds.Length;
            resultBuffer[resultLength++] = 's';
        }

        if (resultLength == 0)
        {
            _spanFormatDefault.CopyTo(resultBuffer);
            resultLength += _spanFormatDefault.Length;
        }

        return new(resultBuffer[..resultLength]);
    }
}
