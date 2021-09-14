using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Properties;
using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.Twitch
{
    public static class TwitchConfig
    {
        public const int AfkCooldown = 10000;
        public const string EmoteInFront = "Okayeg";
        public const int MaxEmoteInFrontLength = 20;
        public const int MaxMessageLength = 500;
        public const int MaxPrefixLength = 10;
        public const int MaxReminders = 10;
        public const int MinimumDelayBetweenMessages = 1300;
        public const string Suffix = "eg";

        public static List<string> Channels => DataBase.GetChannels();

        public static List<string> Owners => new JsonController().BotData.UserLists.Owners;

        public static List<string> Moderators => new JsonController().BotData.UserLists.Moderators;

        public static List<string> SpecialUsers => new JsonController().BotData.UserLists.SpecialUsers;

        public static List<string> SecretUsers => new JsonController().BotData.UserLists.SecretUsers;

        public static List<string> NotAllowedChannels => Resources.NotAllowedChannels.Split().ToList();

        public static List<string> NotLoggedChannels => Resources.NotLoggedChannels.Split().ToList();
    }
}
