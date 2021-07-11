namespace OkayegTeaTimeCSharp.Utils
{
    public class Pattern
    {
        public const string ReminderInTimePattern = @"\s\w+\sin\s(" + TimeSplitPattern + @"(\s|$))+(\S+|\s)*";
        public const string TimeSplitPattern = @"(\d+(y(ear)?|d(ay)?|h(our)?|m(in(ute)?)?|s(ec(ond)?)?)s?)";
        public const string SpotifyLinkPattern = @"(https?://)?open\.spotify\.\w+/track/\w+\?si=\S+";
        public const string SpotifyUriPattern = @"spotify:track:\w{22}";
    }
}