using OkayegTeaTimeCSharp.Properties;
using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.Twitch
{
    public static class Config
    {
        public const string Owner = "strbhlfe";

        public static List<string> GetChannels()
        {
            return Resources.Channels.Split(" ").ToList();
        }

        public static List<string> GetNotLoggedChannels()
        {
            return Resources.NotLoggedChannels.Split(" ").ToList();
        }
    }
}
