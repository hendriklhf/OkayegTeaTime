using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Properties;
using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.Twitch
{
    public static class Config
    {
        public const int AfkCooldown = 10000;
        public const string EmoteInFront = "Okayeg";
        public const int MaxMessageLength = 500;
        public const string Suffix = "eg";

        public static List<string> GetChannels()
        {
            return new OkayegTeaTimeContext().Bots.Where(b => b.Id == 1).FirstOrDefault().Channels.Split().ToList();
        }

        public static List<string> GetNotAllowedChannels()
        {
            return Resources.NotAllowedChannels.Split().ToList();
        }

        public static List<string> GetNotLoggedChannels()
        {
            return Resources.NotLoggedChannels.Split().ToList();
        }
    }
}