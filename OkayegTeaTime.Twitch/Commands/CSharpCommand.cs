using System.Text.RegularExpressions;
using System.Web;
using HLE.Http;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class CSharpCommand : Command
{
    public CSharpCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string code = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            Response = $"{ChatMessage.Username}, {GetCSharpOnlineCompilerResult(code)}";
        }
    }

    private static string GetCSharpOnlineCompilerResult(string input)
    {
        string encodedInput = HttpUtility.HtmlEncode(GetCSharpOnlineCompilerTemplate(input));
        HttpPost request = new("https://dotnetfiddle.net/Home/Run", new[]
        {
            ("CodeBlock", encodedInput),
            ("Compiler", "NetCore22"),
            ("Language", "CSharp"),
            ("ProjectType", "Console"),
            ("NuGetPackageVersionIds", "103505")
        });
        string? result = request.IsValidJsonData ? request.Data.GetProperty("ConsoleOutput").GetString() : "compiler service error";
        return !result?.IsNullOrEmptyOrWhitespace() == true ? (result!.Length > 450 ? $"{result[..450]}..." : result).NewLinesToSpaces() : "executed successfully";
    }

    private static string GetCSharpOnlineCompilerTemplate(string code)
    {
        return ResourceController.CompilerTemplateCSharp.Replace("{code}", code);
    }
}
