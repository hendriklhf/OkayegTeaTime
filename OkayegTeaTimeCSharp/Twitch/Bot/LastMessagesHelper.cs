using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class LastMessagesHelper
    {
        public static void AddChannel(string channel, string message)
        {
            TwitchBot.LastMessages.Add($"#{channel.RemoveHashtag()}", message);
        }

        public static Dictionary<string, string> FillDictionary()
        {
            Dictionary<string, string> dic = new();
            Config.Channels.ForEach(channel =>
            {
                dic.Add($"#{channel}", "");
            });
            return dic;
        }

        public static string GetLastMessage(string channel, string message)
        {
            if (TwitchBot.LastMessages.TryGetValue($"#{channel.RemoveHashtag()}", out string lastMessage))
            {
                return lastMessage;
            }
            else
            {
                AddChannel(channel, message);
                return string.Empty;
            }
        }
        public static void SetLastMessage(string channel, string message)
        {
            TwitchBot.LastMessages[$"#{channel.RemoveHashtag()}"] = message;
        }
    }
}
