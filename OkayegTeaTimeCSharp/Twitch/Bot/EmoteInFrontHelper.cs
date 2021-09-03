using OkayegTeaTimeCSharp.Database;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class EmoteInFrontHelper
    {
        public static void Add(string channel)
        {
            TwitchBot.EmoteInFront.Add(channel, null);
        }

        public static Dictionary<string, string> FillDictionary()
        {
            return DataBase.GetEmotesInFront();
        }

        public static string GetEmote(string channel)
        {
            if (TwitchBot.EmoteInFront.TryGetValue(channel, out string emote))
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
            if (TwitchBot.EmoteInFront.ContainsKey(channel))
            {
                TwitchBot.EmoteInFront[channel] = emote;
            }
            else
            {
                TwitchBot.EmoteInFront.Add(channel, emote);
            }
        }
    }
}
