using System.Text.Json;
using System.Web;
using HLE.Collections;
using HLE.HttpRequests;
using HLE.Strings;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Models.Enums;

namespace OkayegTeaTime.HttpRequests;

public static class HttpRequest
{
    private static readonly string[] _chatRoles =
    {
        "broadcaster", "vips", "moderators", "staff", "admins", "global_mods", "viewers"
    };

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
