using System.Text.Json;
using System.Web;
using HLE.Collections;
using HLE.HttpRequests;
using HLE.Strings;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.JsonClasses.HttpRequests;
using OkayegTeaTime.Logging;
using OkayegTeaTime.Twitch.Api;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Models.Enums;

namespace OkayegTeaTime.HttpRequests;

public static class HttpRequest
{
    public const string FfzSetIdReplacement = "mainSet";

    private static readonly string[] _chatRoles =
    {
        "broadcaster",
        "vips",
        "moderators",
        "staff",
        "admins",
        "global_mods",
        "viewers"
    };

    private static void NormalizeCount<T>(IEnumerable<T>? collection, ref int count)
    {
        if (collection is not null)
        {
            count = Math.Abs(count);
            count = collection.Count() >= count ? count : collection.Count();
        }
    }

    public static IEnumerable<SevenTvEmote>? GetSevenTvEmotes(string channel, int count)
    {
        IEnumerable<SevenTvEmote>? emotes = GetSevenTvEmotes(channel);
        NormalizeCount(emotes, ref count);
        return emotes?.Take(count);
    }

    public static IEnumerable<SevenTvEmote>? GetSevenTvEmotes(string channel)
    {
        return GetSevenTvRequest(channel)?.Data?.User?.Emotes?.Reverse<SevenTvEmote>();
    }

    public static SevenTvRequest? GetSevenTvRequest(string channel)
    {
        try
        {
            HttpPost request = new("https://api.7tv.app/v2/gql",
            new()
            {
                new("query", "{user(id: \"" + channel + "\") {...FullUser}}fragment FullUser on User {id,email, display_name, login,description,role {id,name,position,color,allowed,denied},emotes { id, name, status, visibility, width, height },owned_emotes { id, name, status, visibility, width, height },emote_ids,editor_ids,editors {id, display_name, login,role { id, name, position, color, allowed, denied },profile_image_url,emote_ids},editor_in {id, display_name, login,role { id, name, position, color, allowed, denied },profile_image_url,emote_ids},twitch_id,broadcaster_type,profile_image_url,created_at}"),
                new("variables", "{}")
            });

            if (request.Result is null)
            {
                return null;
            }
            return JsonSerializer.Deserialize<SevenTvRequest>(request.Result);
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            return null;
        }
    }

    public static IEnumerable<BttvSharedEmote>? GetBttvEmotes(string channel, int count)
    {
        IEnumerable<BttvSharedEmote>? emotes = GetBttvEmotes(channel);
        NormalizeCount(emotes, ref count);
        return emotes?.Take(count);
    }

    public static IEnumerable<BttvSharedEmote>? GetBttvEmotes(string channel)
    {
        return GetBttvRequest(channel)?.SharedEmotes?.Reverse<BttvSharedEmote>();
    }

    public static int GetChatterCount(string channel)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel.Remove("#")}/chatters");
        if (request.Data is null)
        {
            return 0;
        }

        return request.Data.Value.GetProperty("chatter_count").GetInt32();
    }

    public static IEnumerable<Chatter> GetChatters(string channel)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel.Remove("#")}/chatters");
        List<Chatter> result = new();
        if (request.Data is null)
        {
            return result;
        }

        JsonElement chatters = request.Data.Value.GetProperty("chatters");
        byte pIdx = 0;
        _chatRoles.ForEach(p =>
        {
            JsonElement chatterList = chatters.GetProperty(p);
            for (int i = 0; i < chatterList.GetArrayLength(); i++)
            {
                result.Add(new(chatterList[i].GetString()!, (ChatRole)pIdx));
            }
            pIdx++;
        });
        return result;
    }

    public static IEnumerable<FfzEmote>? GetFfzEmotes(string channel, int count)
    {
        IEnumerable<FfzEmote>? emotes = GetFfzEmotes(channel);
        NormalizeCount(emotes, ref count);
        return emotes?.Take(count);
    }

    public static IEnumerable<FfzEmote>? GetFfzEmotes(string channel)
    {
        return GetFfzRequest(channel)?.Set?.EmoteSet?.Emotes?.Reverse<FfzEmote>();
    }

    public static FfzRequest? GetFfzRequest(string channel)
    {
        try
        {
            HttpGet request = new($"https://api.frankerfacez.com/v1/room/{channel.Remove("#").ToLower()}");
            if (request.Data is null || request.Result is null)
            {
                return null;
            }

            int setId = request.Data.Value.GetProperty("room").GetProperty("set").GetInt32();
            string result = request.Result.Replace($"\"{setId}\":", $"\"{FfzSetIdReplacement}\":");
            return JsonSerializer.Deserialize<FfzRequest>(result);
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            return null;
        }
    }

    public static string GetMathResult(string expression)
    {
        HttpGet request = new($"http://api.mathjs.org/v4/?expr={HttpUtility.UrlEncode(expression)}");
        return request.IsValidJsonData && request.Data is not null ? request.Data.Value.GetRawText() : request.Result ?? "api error";
    }

    public static string GetCSharpOnlineCompilerResult(string input)
    {
        const string errorMessage = "Compiler service error";
        string? encodedInput = HttpUtility.HtmlEncode(GetCSharpOnlineCompilerTemplate(input));
        if (encodedInput is null)
        {
            return errorMessage;
        }

        HttpPost request = new("https://dotnetfiddle.net/Home/Run",
            new()
            {
                new("CodeBlock", encodedInput),
                new("Compiler", "NetCore22"),
                new("Language", "CSharp"),
                new("ProjectType", "Console")
            });
        string? result = request.IsValidJsonData && request.Data is not null ? request.Data.Value.GetProperty("ConsoleOutput").GetString() : errorMessage;
        if (!result?.IsNullOrEmptyOrWhitespace() == true)
        {
            if (result!.Length > 500)
            {
                return result[450..].NewLinesToSpaces();
            }
            else
            {
                return result!;
            }
        }
        else
        {
            return "compiled successfully";
        }
    }

    private static string? GetCSharpOnlineCompilerTemplate(string code)
    {
        return FileController.OnlineCompilerTemplateCSharp?.Replace("{code}", code);
    }

    public static BttvRequest? GetBttvRequest(string channel)
    {
        int? id = TwitchApi.GetUserId(channel);
        if (id.HasValue)
        {
            return GetBttvRequest(id.Value);
        }
        return null;
    }

    public static BttvRequest? GetBttvRequest(int channelId)
    {
        try
        {
            HttpGet request = new($"https://api.betterttv.net/3/cached/users/twitch/{channelId}");
            if (request.Result is null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<BttvRequest>(request.Result);
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            return null;
        }
    }

    public static string? GetGoLangOnlineCompilerResult(string code)
    {
        HttpPost request = new("https://play.golang.org/compile",
            new()
            {
                new("version", "2"),
                new("body", GetGoLangOnlineCompilerTemplate(code)),
                new("withVet", "true")
            });
        if (request.Data is null)
        {
            return null;
        }

        string? error = request.Data.Value.GetProperty("Errors").GetString();
        bool hasError = !string.IsNullOrEmpty(error);
        if (!hasError)
        {
            return request.Data.Value.GetProperty("Events")[0].GetProperty("Message").GetString();
        }
        else
        {
            return error;
        }
    }

    private static string GetGoLangOnlineCompilerTemplate(string code)
    {
        return FileController.OnlineCompilerTemplateGo.Replace("{code}", code);
    }
}
