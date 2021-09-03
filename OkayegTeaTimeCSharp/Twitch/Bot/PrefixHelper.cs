using OkayegTeaTimeCSharp.Database;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class PrefixHelper
    {
        public static void Add(string channel)
        {
            TwitchBot.Prefixes.Add(channel, DataBase.GetPrefix(channel));
        }

        public static Dictionary<string, string> FillDictionary()
        {
            return DataBase.GetPrefixes();
        }

        public static string GetPrefix(string channel)
        {
            return TwitchBot.Prefixes.TryGetValue(channel, out string prefix) ? prefix : string.Empty;
        }

        public static void Update(string channel)
        {
            TwitchBot.Prefixes[channel] = DataBase.GetPrefix(channel);
        }
    }
}
