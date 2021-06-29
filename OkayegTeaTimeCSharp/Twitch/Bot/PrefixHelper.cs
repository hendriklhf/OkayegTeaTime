using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class PrefixHelper
    {
        public static Dictionary<string, string> FillDictionary()
        {
            return DataBase.GetPrefixes();
        }

        public static void Add(string channel)
        {
            TwitchBot.Prefixes.Add($"#{channel.ReplaceHashtag()}", DataBase.GetPrefix(channel));
        }

        public static void Update(string channel)
        {
            TwitchBot.Prefixes[$"#{channel.ReplaceHashtag()}"] = DataBase.GetPrefix(channel);
        }

        public static string GetPrefix(string channel)
        {
            return TwitchBot.Prefixes.TryGetValue($"#{channel}", out string prefix) ? prefix : string.Empty;
        }
    }
}
