using System.Text.RegularExpressions;
using HLE.Text;

namespace OkayegTeaTime.Utils;

public static partial class Pattern
{
    public static Regex MultipleTargets => GetMultipleTargetsRegex();

    public static Regex SpotifyLink => GetSpotifyLinkRegex();

    public static Regex SpotifyUri => GetSpotifyUriRegex();

    public static Regex SpotifyId => GetSpotifyIdRegex();

    public static Regex SpotifyTrackSlash => GetSpotifyTrackSlashRegex();

    public static Regex PajaAlert => GetPajaAlertRegex();

    [GeneratedRegex(@"\w+(,\s?\w+)*", RegexOptions.Compiled, 250)]
    private static partial Regex GetMultipleTargetsRegex();

    [GeneratedRegex(@"(https?://)?open\.spotify\.\w+(/.+)?/track/\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled, 250)]
    private static partial Regex GetSpotifyLinkRegex();

    [GeneratedRegex(@"spotify:track:\w{22}", RegexOptions.IgnoreCase | RegexOptions.Compiled, 250)]
    private static partial Regex GetSpotifyUriRegex();

    [GeneratedRegex(@"^\w{22}$", RegexOptions.Compiled, 250)]
    private static partial Regex GetSpotifyIdRegex();

    [GeneratedRegex(@"track/\w+", RegexOptions.Compiled, 250)]
    private static partial Regex GetSpotifyTrackSlashRegex();

    [GeneratedRegex($@"^\s*pajaS\s+{Emoji.RotatingLight}\s+ALERT\s*$", RegexOptions.Compiled)]
    private static partial Regex GetPajaAlertRegex();
}
