using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.HttpRequests
{
    public static class HttpRequest
    {
        public static int GetChatterCount(string channel)
        {
            HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel.Replace("#", "")}/chatters");
            return request.Data.GetProperty("chatter_count").GetInt32();
        }

        public static List<string> GetChatters(string channel)
        {
            List<string> result = new();
            HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel.Replace("#", "")}/chatters");
            return result
                .Concat(request.Data.GetProperty("chatters").GetProperty("broadcaster").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("vips").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("moderators").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("staff").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("admins").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("global_mods").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("viewers").ToString().WordArrayStringToList())
                .ToList();
        }
    }
}
