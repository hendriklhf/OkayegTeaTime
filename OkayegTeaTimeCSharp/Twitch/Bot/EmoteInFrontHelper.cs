using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class EmoteInFrontHelper
    {
        public static Dictionary<string, string> FillDictionary()
        {
            return DataBase.GetEmotesInFront();
        }

        public static string GetEmote(string channel)
        {
            if (TwitchBot.EmoteInFront.TryGetValue($"#{channel.RemoveHashtag()}", out string emote))
            {
                return !string.IsNullOrEmpty(emote) ? emote : Config.EmoteInFront;
            }
            else
            {
                return Config.EmoteInFront;
            }
        }

        public static void Update(string channel, string emote)
        {
            if (TwitchBot.EmoteInFront.ContainsKey($"#{channel.RemoveHashtag()}"))
            {
                TwitchBot.EmoteInFront[$"#{channel.RemoveHashtag()}"] = emote;
            }
            else
            {
                TwitchBot.EmoteInFront.Add($"#{channel.RemoveHashtag()}", emote);
            }
        }
    }
}
