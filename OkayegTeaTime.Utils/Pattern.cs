namespace OkayegTeaTime.Utils;

public static class Pattern
{
    public const string MultipleTargets = @"\w+(,\s?\w+)*";
    public const string SpotifyLink = @"(https?://)?open\.spotify\.\w+/track/\w+";
    public const string SpotifyUri = @"spotify:track:\w{22}";
    public const string SpotifyCodeUrl = @"(https?://(www\.)?)?example\.com/callback\?code=\S+";
}
