using System.IO;
using System.Text.Json;
using System.Web;
using HLE.Collections;
using HLE.HttpRequests;
using HLE.Strings;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests;
using OkayegTeaTimeCSharp.Logging;
using OkayegTeaTimeCSharp.Models;
using OkayegTeaTimeCSharp.Models.Enums;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Utils;
using Path = OkayegTeaTimeCSharp.Properties.Path;

namespace OkayegTeaTimeCSharp.HttpRequests
{
    public static class HttpRequest
    {
        public static List<Emote> Get7TVEmotes(string channel, int count)
        {
            try
            {
                List<Emote> result = Get7TVEmotes(channel);
                count = result.Count >= count ? count : result.Count;
                return result.Take(count).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Emote> Get7TVEmotes(string channel)
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
                JsonElement jEmotes = request.Data.GetProperty("data").GetProperty("user").GetProperty("emotes");
                for (int i = 0; i <= jEmotes.GetArrayLength() - 1; i++)
                {
                    emotes.Add(new(i, jEmotes[i].GetProperty("name").GetString()));
                }
                return emotes.OrderByDescending(e => e.Index).ToList();
            }
            catch (InvalidOperationException ex)
            {
                Logger.Log(ex);
                return new();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IEnumerable<BttvSharedEmote> GetBTTVEmotes(string channel, int count)
        {
            return GetBTTVEmotes(channel)?.Take(count);
        }

        public static IEnumerable<BttvSharedEmote> GetBTTVEmotes(string channel)
        {
            return GetBttvRequest(channel)?.SharedEmotes?.Reverse<BttvSharedEmote>();
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
            new string[] { "broadcaster", "vips", "moderators", "staff", "admins", "global_mods", "viewers" }.ForEach(p =>
            {
                JsonElement chatterList = chatters.GetProperty(p);
                for (int i = 0; i < chatterList.GetArrayLength(); i++)
                {
                    result.Add(new(chatterList[i].GetString(), (ChatRole)i)); //error: i needs to be the index of p
                }
            });
            return result;
        }

        public static List<Emote> GetFFZEmotes(string channel, int count)
        {
            try
            {
                List<Emote> result = GetFFZEmotes(channel);
                count = result.Count >= count ? count : result.Count;
                return result.Take(count).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Emote> GetFFZEmotes(string channel)
        {
            try
            {
                List<Emote> emotes = new();
                HttpGet request = new($"https://api.frankerfacez.com/v1/room/{channel.RemoveHashtag()}");
                int setID = request.Data.GetProperty("room").GetProperty("set").GetInt32();
                int emoteCountInChannel = request.Data.GetProperty("sets").GetProperty(setID.ToString()).GetProperty("emoticons").GetArrayLength();
                for (int i = 0; i <= emoteCountInChannel - 1; i++)
                {
                    emotes.Add(new(i, request.Data.GetProperty("sets").GetProperty($"{setID}").GetProperty("emoticons")[i].GetProperty("name").GetString()));
                }
                return emotes.OrderByDescending(e => e.Index).ToList();
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
            return File.ReadAllText(Path.OnlineCompilerTemplate).Replace("{code}", code);
        }

        public static BttvRequest GetBttvRequest(string channel)
        {
            return GetBttvRequest(new TwitchAPI().GetChannelID(channel).ToInt());
        }

        public static BttvRequest GetBttvRequest(int channelId)
        {
            try
            {
                HttpGet request = new($"https://api.betterttv.net/3/cached/users/twitch/{channelId}");
                return JsonSerializer.Deserialize<BttvRequest>(request.Result);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
