using System.Text.Json;
using System.Web;
using HLE.Collections;
using HLE.HttpRequests;
using HLE.Strings;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Models.Enums;

namespace OkayegTeaTime.HttpRequests;

public static class HttpRequest
{
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

    public static int GetChatterCount(string channel)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel}/chatters");
        if (!request.IsValidJsonData)
        {
            return 0;
        }

        return request.Data.GetProperty("chatter_count").GetInt32();
    }

    public static IEnumerable<Chatter> GetChatters(string channel)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel}/chatters");
        List<Chatter> result = new();
        if (!request.IsValidJsonData)
        {
            return result;
        }

        JsonElement chatters = request.Data.GetProperty("chatters");
        byte pIdx = 0;
        _chatRoles.ForEach(p =>
        {
            JsonElement chatterList = chatters.GetProperty(p);
            for (int i = 0; i < chatterList.GetArrayLength(); i++)
            {
                result.Add(new(chatterList[i].GetString()!, (ChatRole)pIdx++));
            }
        });
        return result;
    }

    public static string GetMathResult(string expression)
    {
        HttpGet request = new($"https://api.mathjs.org/v4/?expr={HttpUtility.UrlEncode(expression)}");
        return request.Result ?? "api error";
    }

    public static string GetCSharpOnlineCompilerResult(string input)
    {
        string encodedInput = HttpUtility.HtmlEncode(GetCSharpOnlineCompilerTemplate(input));

        HttpPost request = new("https://dotnetfiddle.net/Home/Run", new[]
        {
            ("CodeBlock", encodedInput),
            ("Compiler", "NetCore22"),
            ("Language", "CSharp"),
            ("ProjectType", "Console")
        });
        string? result = request.IsValidJsonData ? request.Data.GetProperty("ConsoleOutput").GetString() : "compiler service error";
        if (!result?.IsNullOrEmptyOrWhitespace() == true)
        {
            return (result!.Length > 450 ? $"{result[..450]}..." : result).NewLinesToSpaces();
        }

        return "compiled successfully";
    }

    private static string GetCSharpOnlineCompilerTemplate(string code)
    {
        return ResourceController.CompilerTemplateCSharp.Replace("{code}", code);
    }

    public static string? GetGoLangOnlineCompilerResult(string code)
    {
        HttpPost request = new("https://play.golang.org/compile", new[]
        {
            ("version", "2"),
            ("body", GetGoLangOnlineCompilerTemplate(code)),
            ("withVet", "true")
        });
        if (!request.IsValidJsonData)
        {
            return null;
        }

        string error = request.Data.GetProperty("Errors").GetString()!;
        bool hasError = !string.IsNullOrEmpty(error);
        string result = hasError ? error : request.Data.GetProperty("Events")[0].GetProperty("Message").GetString()!;
        return (result.Length > 450 ? $"{result[..450]}..." : result).NewLinesToSpaces();
    }

    private static string GetGoLangOnlineCompilerTemplate(string code)
    {
        return ResourceController.CompilerTemplateGo.Replace("{code}", code);
    }

    public static string GetCppOnlineCompilerResult(string code)
    {
        HttpPost request = new("https://tpcg2.tutorialspoint.com/tpcg.php", new[]
        {
            ("lang", "cpp"),
            ("device", string.Empty),
            ("code", GetCppOnlineCompilerTemplate(code)),
            ("stdinput", string.Empty),
            ("ext", "cpp"),
            ("compile", HttpUtility.HtmlEncode("g++ -o main *.cpp")),
            ("execute", "main"),
            ("mainfile", "main.cpp"),
            ("uid", "968291")
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

        return (result.Length > 450 ? $"{result[450..]}..." : result).NewLinesToSpaces();
    }

    private static string GetCppOnlineCompilerTemplate(string code)
    {
        return ResourceController.CompilerTemplateCpp.Replace("{code}", code);
    }
}
