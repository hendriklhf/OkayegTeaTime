namespace OkayegTeaTimeCSharp.Utils;

public class Pattern
{
    public const string ReminderInTime = $@"\s{MultipleReminderTargets}\sin\s({TimeSplit}(\s|$))+(\S+|\s)*";
    public const string MultipleReminderTargets = @"\w+(,\s?\w+)*";
    public const string TimeSplit = @"(\d+(y(ear)?|d(ay)?|h(our)?|m(in(ute)?)?|s(ec(ond)?)?)s?)";
    public const string SpotifyLink = @"(https?://)?open\.spotify\.\w+/track/\w+\?si=\S+";
    public const string SpotifyUri = @"spotify:track:\w{22}";
    public const string SearchUserParameter = @"\s?(-u|--user)\s\w+";
    public const string SearchChannelParameter = @"\s?(-c|--channel)\s#?\w+";
}
