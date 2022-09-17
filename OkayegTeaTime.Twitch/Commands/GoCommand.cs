using System.Text.RegularExpressions;
using HLE.Http;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Go)]
public sealed class GoCommand : Command
{
    public GoCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string code = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            string? response = GetGoLangOnlineCompilerResult(code);
            if (response is null)
            {
                Response = "compiler service error";
                return;
            }

            Response = $"{ChatMessage.Username}, {response}";
        }
    }

    private static string? GetGoLangOnlineCompilerResult(string code)
    {
        HttpPost request = new("https://play.golang.org/compile", new[]
        {
            ("version", "2"),
            ("body", ResourceController.CompilerTemplateGo.Replace("{code}", code)),
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
}
