using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Properties;
using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.Twitch
{
    public static class Config
    {
        public const int AfkCooldown = 10000;
        public const string EmoteInFront = "Okayeg";
        public const int MaxEmoteInFrontLength = 20;
        public const int MaxReminders = 10;
        public const int MaxMessageLength = 500;
        public const int MaxPrefixLength = 10;
        public const int MinimumDelayBetweenMessages = 1300;
        public const string Suffix = "eg";

        public static readonly List<string> Channels = DataBase.GetChannels();

        public static readonly List<string> NotAllowedChannels = Resources.NotAllowedChannels.Split().ToList();

        public static readonly List<string> NotLoggedChannels = Resources.NotLoggedChannels.Split().ToList();
    }
}
