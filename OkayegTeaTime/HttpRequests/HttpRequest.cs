using System.Text.Json;
using System.Web;
using HLE.Collections;
using HLE.HttpRequests;
using HLE.Strings;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Jsons.HttpRequests.Bttv;
using OkayegTeaTime.Files.Jsons.HttpRequests.Ffz;
using OkayegTeaTime.Files.Jsons.HttpRequests.SevenTv;
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
        if (collection is null)
        {
            return;
        }

        count = Math.Abs(count);
        T[] arr = collection.ToArray();
        count = arr.Length >= count ? count : arr.Length;
    }

    public static IEnumerable<SevenTvEmote>? GetSevenTvEmotes(string channel, int count)
    {
        IEnumerable<SevenTvEmote>? emotes = GetSevenTvEmotes(channel);
        NormalizeCount(emotes, ref count);
        return emotes?.Take(count);
    }

    public static IEnumerable<SevenTvEmote>? GetSevenTvEmotes(string channel)
    {
        return GetSevenTvRequest(channel)?.Data?.User?.Emotes;
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
        return GetFfzRequest(channel)?.Set?.EmoteSet?.Emotes;
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
        HttpGet request = new($"https://api.mathjs.org/v4/?expr={HttpUtility.UrlEncode(expression)}");
        return request.IsValidJsonData && request.Data is not null ? request.Data.Value.GetRawText() : request.Result ?? "api error";
    }

    public static string GetCSharpOnlineCompilerResult(string input)
    {
        string? encodedInput = HttpUtility.HtmlEncode(GetCSharpOnlineCompilerTemplate(input));
        if (encodedInput is null)
        {
            return "HTTP encoding error";
        }

        HttpPost request = new("https://dotnetfiddle.net/Home/Run",
            new()
            {
                new("CodeBlock", encodedInput),
                new("Compiler", "NetCore22"),
                new("Language", "CSharp"),
                new("ProjectType", "Console")
            });
        string? result = request.IsValidJsonData && request.Data is not null ? request.Data.Value.GetProperty("ConsoleOutput").GetString() : "compiler service error";
        if (!result?.IsNullOrEmptyOrWhitespace() == true)
        {
            return result!.Length > 500 ? result[450..].NewLinesToSpaces() : result;
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
        return id.HasValue ? GetBttvRequest(id.Value) : null;
    }

    public static BttvRequest? GetBttvRequest(int channelId)
    {
        try
        {
            HttpGet request = new($"https://api.betterttv.net/3/cached/users/twitch/{channelId}");
            return request.Result is null ? null : JsonSerializer.Deserialize<BttvRequest>(request.Result);
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
        return hasError ? error : request.Data.Value.GetProperty("Events")[0].GetProperty("Message").GetString();
    }

    private static string GetGoLangOnlineCompilerTemplate(string code)
    {
        return FileController.OnlineCompilerTemplateGo.Replace("{code}", code);
    }

    public static string GetCppOnlineCompilerResult(string code)
    {
        HttpPost request = new("https://tpcg2.tutorialspoint.com/tpcg.php", new List<KeyValuePair<string, string>>
        {
            new("lang", "cpp"),
            new("device", string.Empty),
            new("code", GetCppOnlineCompilerTemplate(code)),
            new("stdinput", string.Empty),
            new("ext", "cpp"),
            new("compile", HttpUtility.HtmlEncode("g++ -o main *.cpp")),
            new("execute", "main"),
            new("mainfile", "main.cpp"),
            new("uid", "968291")
        });

        if (request.Result is null)
        {
            return "compiler service error";
        }

        string result = request.Result.NewLinesToSpaces().Match(@"\$main(</b>|</span>|<br>){3}.*")[20..].TrimAll();
        if (result.IsNullOrEmptyOrWhitespace())
        {
            return "compiled successfully";
        }
        else
        {
            return result.Length > 500 ? $"{result[450..]}..." : result;
        }
    }

    private static string GetCppOnlineCompilerTemplate(string code)
    {
        return FileController.OnlineCompilerTemplateCpp.Replace("{code}", code);
    }
}
