using System.Text.RegularExpressions;
using System.Web;
using HLE.Http;
using OkayegTeaTime.Files;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.CSharp)]
public sealed class CSharpCommand : Command
{
    public CSharpCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string code = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            Response = $"{ChatMessage.Username}, {GetProgramOutput(code)}";
        }
    }

    private static string GetProgramOutput(string input)
    {
        string encodedInput = HttpUtility.HtmlEncode(ResourceController.CSharpTemplate.Replace("{code}", input));
        HttpPost request = new("https://dotnetfiddle.net/Home/Run", new[]
        {
            ("CodeBlock", encodedInput),
            ("Compiler", "NetCore22"),
            ("Language", "CSharp"),
            ("ProjectType", "Console"),
            ("NuGetPackageVersionIds", AppSettings.HleNugetVersionId)
        });
        string? result = request.IsValidJsonData ? request.Data.GetProperty("ConsoleOutput").GetString() : "compiler service error";
        return !string.IsNullOrWhiteSpace(result) ? (result.Length > 450 ? $"{result[..450]}..." : result).NewLinesToSpaces() : "executed successfully";
    }
}
