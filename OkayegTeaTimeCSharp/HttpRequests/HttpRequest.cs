using HLE.HttpRequests;
using OkayegTeaTimeCSharp.HttpRequests.Enums;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace OkayegTeaTimeCSharp.HttpRequests
{
    public static class HttpRequest
    {
        public static List<Emote> Get7TVEmotes(string channel, int count = 5)
        {
            try
            {
                List<Emote> emotes = new();
                HttpPost request = new("https://api.7tv.app/v2/gql",
                   new()
                   {
                       new("query", "{user(id: \"" + channel + "\") {...FullUser}}fragment FullUser on User {id,email, display_name, login,description,role {id,name,position,color,allowed,denied},emotes { id, name, status, visibility, width, height },owned_emotes { id, name, status, visibility, width, height },emote_ids,editor_ids,editors {id, display_name, login,role { id, name, position, color, allowed, denied },profile_image_url,emote_ids},editor_in {id, display_name, login,role { id, name, position, color, allowed, denied },profile_image_url,emote_ids},twitch_id,broadcaster_type,profile_image_url,created_at}"),
                       new("variables", "{}")
                   });
                int emoteCountInChannel = request.Data.GetProperty("data").GetProperty("user").GetProperty("emotes").GetArrayLength();
                count = count > emoteCountInChannel ? emoteCountInChannel : (count <= 0 ? 5 : count);
                for (int i = 0; i <= emoteCountInChannel - 1; i++)
                {
                    emotes.Add(new(i, request.Data.GetProperty("data").GetProperty("user").GetProperty("emotes")[i].GetProperty("name").GetString()));
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
                count = count > emoteCountInChannel ? emoteCountInChannel : (count <= 0 ? 5 : count);
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

        public static int GetChatterCount(string channel)
        {
            HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel.RemoveHashtag()}/chatters");
            return request.Data.GetProperty("chatter_count").GetInt32();
        }

        public static List<Chatter> GetChatters(string channel)
        {
            HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel.RemoveHashtag()}/chatters");
            JsonElement chatters = request.Data.GetProperty("chatters");
            List<Chatter> result = new();
            chatters.GetProperty("broadcaster").ToString().WordArrayStringToList().ForEach(c => result.Add(new(c, ChatRole.Broadcaster)));
            chatters.GetProperty("vips").ToString().WordArrayStringToList().ForEach(c => result.Add(new(c, ChatRole.VIP)));
            chatters.GetProperty("moderators").ToString().WordArrayStringToList().ForEach(c => result.Add(new(c, ChatRole.Moderator)));
            chatters.GetProperty("staff").ToString().WordArrayStringToList().ForEach(c => result.Add(new(c, ChatRole.Staff)));
            chatters.GetProperty("admins").ToString().WordArrayStringToList().ForEach(c => result.Add(new(c, ChatRole.Admin)));
            chatters.GetProperty("global_mods").ToString().WordArrayStringToList().ForEach(c => result.Add(new(c, ChatRole.GlobalMod)));
            chatters.GetProperty("viewers").ToString().WordArrayStringToList().ForEach(c => result.Add(new(c, ChatRole.Viewer)));
            return result;
        }

        public static List<Emote> GetFFZEmotes(string channel, int count = 5)
        {
            try
            {
                List<Emote> emotes = new();
                HttpGet request = new($"https://api.frankerfacez.com/v1/room/{channel.RemoveHashtag()}");
                int setID = request.Data.GetProperty("room").GetProperty("set").GetInt32();
                int emoteCountInChannel = request.Data.GetProperty("sets").GetProperty(setID.ToString()).GetProperty("emoticons").GetArrayLength();
                count = count > emoteCountInChannel ? emoteCountInChannel : (count <= 0 ? 5 : count);
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

        public static string GetMathResult(string expression)
        {
            HttpGet request = new($"http://api.mathjs.org/v4/?expr={HttpUtility.UrlEncode(expression)}");
            return request.ValidJsonData ? request.Data.GetRawText() : request.Result;
        }

        public static string GetOnlineCompilerResult(string input)
        {
            HttpPost request = new("https://dotnetfiddle.net/Home/Run",
                new()
                {
                    new("CodeBlock", HttpUtility.HtmlEncode(GetOnlineCompilerTemplate(input))),
                    new("Compiler", "NetCore22"),
                    new("Language", "CSharp"),
                    new("ProjectType", "Console")
                });
            return request.ValidJsonData ? request.Data.GetProperty("ConsoleOutput").GetString() : "Compiler service error";
        }

        private static string GetOnlineCompilerTemplate(string code)
        {
            return File.ReadAllText(Resources.OnlineCompilerTemplatePath).Replace("{code}", code);
        }
    }
}
