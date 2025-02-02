using System.Text.RegularExpressions;
using HLE.Text;

namespace OkayegTeaTime.Utils;

public static partial class StringHelper
{
    [GeneratedRegex(@"^#?[a-z\d]\w{2,24}$", RegexOptions.Compiled | RegexOptions.IgnoreCase, 1000)]
    private static partial Regex GetChannelPattern();

    public static string Antiping(this string value) => value.Insert(value.Length >> 1, StringHelpers.AntipingChar);

    public static string NewLinesToSpaces(this string value) => value.ReplaceLineEndings(" ");

    public static bool FormatChannel(ref string channel, bool withHashTag = false)
    {
        if (!GetChannelPattern().IsMatch(channel))
        {
            return false;
        }

        channel = withHashTag switch
        {
            true => channel[0] == '#' ? channel : '#' + channel,
            _ => channel[0] == '#' ? channel[1..] : channel
        };

        channel = channel.ToLowerInvariant();
        return true;
    }
}
