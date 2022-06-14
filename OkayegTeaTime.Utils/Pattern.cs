namespace OkayegTeaTime.Utils;

public static class Pattern
{
    public const string ReminderInTime = $@"\s{MultipleTargets}\sin(\s{TimeSplit})+(\s(\S+|\s)+)?.*";
    public const string MultipleTargets = @"\w+(,\s?\w+)*";
    public const string TimeSplit = @"(\d+(y(ears?)?|d(ays?)?|h(ours?)?|m(in(ute)?s?)?|s(ec(ond)?s?)?))";
    public const string SpotifyLink = @"(https?://)?open\.spotify\.\w+/track/\w+";
    public const string SpotifyUri = @"spotify:track:\w{22}";
    public const string SpotifyCodeUrl = @"(https?://(www\.)?)?example\.com/callback\?code=\S+";
}
