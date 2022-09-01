using System;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Utils;

public static class Pattern
{
    public static Regex MultipleTargets { get; } = new(@"\w+(,\s?\w+)*", RegexOptions.Compiled, TimeSpan.FromMicroseconds(250));

    public static Regex SpotifyLink { get; } = new(@"(https?://)?open\.spotify\.\w+/track/\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMicroseconds(250));

    public static Regex SpotifyUri { get; } = new(@"spotify:track:\w{22}", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMicroseconds(250));
}
