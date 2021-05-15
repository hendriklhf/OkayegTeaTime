using OkayegTeaTimeCSharp.Database.Models;
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
            OkayegTeaTimeContext database = new();
            return database.Bots.Where(b => b.Id == 1).FirstOrDefault().Channels.Split(" ").ToList();
        }

        public static List<string> GetNotLoggedChannels()
        {
            return Resources.NotLoggedChannels.Split(" ").ToList();
        }
    }
}
