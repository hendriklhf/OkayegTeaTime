using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class EmoteInFrontHelper
    {
        public static Dictionary<string, string> FillEmotesInFront()
        {
            return DataBase.GetEmotesInFront();
        }

        public static string GetEmote(string channel)
        {
            if (TwitchBot.EmoteInFront.TryGetValue($"#{channel.ReplaceHashtag()}", out string emote))
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
            if (TwitchBot.EmoteInFront.ContainsKey($"#{channel.ReplaceHashtag()}"))
            {
                TwitchBot.EmoteInFront[$"#{channel.ReplaceHashtag()}"] = emote;
            }
            else
            {
                TwitchBot.EmoteInFront.Add($"#{channel.ReplaceHashtag()}", emote);
            }
        }
    }
}
