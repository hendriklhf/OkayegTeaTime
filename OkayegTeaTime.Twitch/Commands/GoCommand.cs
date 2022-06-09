using System.Text.RegularExpressions;
using HLE.HttpRequests;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class GoCommand : Command
{
    public GoCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string code = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            Response = $"{ChatMessage.Username}, {GetGoLangOnlineCompilerResult(code)}";
        }
    }

    private static string? GetGoLangOnlineCompilerResult(string code)
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
}
