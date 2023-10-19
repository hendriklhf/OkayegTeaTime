using System;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Utils;

public static class StringHelper
{
    private static readonly Regex _channelPattern = new(@"^#?\w{3,25}$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public static string Antiping(this string value) => value.Insert(value.Length >> 1, HLE.Strings.StringHelper.AntipingChar);

    public static string NewLinesToSpaces(this string value) => value.ReplaceLineEndings(" ");

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
        }).ToLowerInvariant();

        return true;
    }
}
