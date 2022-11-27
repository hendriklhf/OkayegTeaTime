using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using OkayegTeaTime.Database;
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
            Response = $"{ChatMessage.Username}, {GetProgramOutput(code).Result}";
        }
    }

    private static async Task<string> GetProgramOutput(string input)
    {
        try
        {
            string codeBlock = HttpUtility.HtmlEncode(ResourceController.CSharpTemplate.Replace("{code}", input));
            using HttpClient httpClient = new();
            using HttpResponseMessage response = await httpClient.PostAsync("https://dotnetfiddle.net/home/run", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("CodeBlock", codeBlock),
                new("Compiler", "Net7"),
                new("Language", "CSharp"),
                new("MvcViewEngine", "Razor"),
                new("NuGetPackageVersionIds", AppSettings.HleNugetVersionId),
                new("OriginalCodeBlock", codeBlock),
                new("OriginalFiddleId", "CsConsCore"),
                new("ProjectType", "Console"),
                new("TimeOffset", "1"),
                new("UseResultCache", "false")
            }));
            string result = await response.Content.ReadAsStringAsync();
            JsonElement json = JsonSerializer.Deserialize<JsonElement>(result);
            string output = json.GetProperty("ConsoleOutput").GetString()!.NewLinesToSpaces();
            return !string.IsNullOrWhiteSpace(output) ? (output.Length > 450 ? $"{output[..450]}..." : output) : "executed successfully";
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return "an unexpected error occurred. The API might not be available.";
        }
    }
}
