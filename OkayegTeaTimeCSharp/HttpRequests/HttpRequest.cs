using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.HttpRequests
{
    public static class HttpRequest
    {
        public static int GetChatterCount(string channel)
        {
            HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel.ReplaceHashtag()}/chatters");
            return request.Data.GetProperty("chatter_count").GetInt32();
        }

        public static List<string> GetChatters(string channel)
        {
            List<string> result = new();
            HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel.ReplaceHashtag()}/chatters");
            return result
                .Concat(request.Data.GetProperty("chatters").GetProperty("broadcaster").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("vips").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("moderators").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("staff").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("admins").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("global_mods").ToString().WordArrayStringToList())
                .Concat(request.Data.GetProperty("chatters").GetProperty("viewers").ToString().WordArrayStringToList())
                .Where(user => !JsonHelper.BotData.UserLists.SpecialUsers.Contains(user))
                .ToList();
        }

        public static List<Emote> GetFFZEmotes(string channel, int count = 5)
        {
            try
            {
                List<Emote> emotes = new();
                HttpGet request = new($"https://api.frankerfacez.com/v1/room/{channel.ReplaceHashtag()}");
                int setID = request.Data.GetProperty("room").GetProperty("set").GetInt32();
                int emoteCountInChannel = request.Data.GetProperty("sets").GetProperty(setID.ToString()).GetProperty("emoticons").GetArrayLength();
                count = count > emoteCountInChannel ? emoteCountInChannel : (count == 0 ? 1 : count);
                for (int i = 0; i <= emoteCountInChannel - 1; i++)
                {
                    emotes.Add(new(i, request.Data.GetProperty("sets").GetProperty(setID.ToString()).GetProperty("emoticons")[i].GetProperty("name").GetString()));
                }
                return emotes.OrderByDescending(e => e.Index).Take(count).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Emote> GetBTTVEmotes(string channel, int count = 5)
        {
            try
            {
                List<Emote> emotes = new();
                HttpGet request = new($"https://api.betterttv.net/3/cached/users/twitch/{TwitchAPI.GetChannelID(channel)}");
                int emoteCountInChannel = request.Data.GetProperty("sharedEmotes").GetArrayLength();
                count = count > emoteCountInChannel ? emoteCountInChannel : (count == 0 ? 1 : count);

                for (int i = 0; i <= emoteCountInChannel - 1; i++)
                {
                    emotes.Add(new(i, request.Data.GetProperty("sharedEmotes")[i].GetProperty("code").GetString()));
                }
                return emotes.OrderByDescending(e => e.Index).Take(count).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}