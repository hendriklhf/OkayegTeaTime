using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class LastMessagesHelper
    {
        public static Dictionary<string, string> FillDictionary()
        {
            Dictionary<string, string> dic = new();
            Config.GetChannels().ForEach(channel =>
            {
                dic.Add($"#{channel}", "");
            });
            return dic;
        }
    }
}
