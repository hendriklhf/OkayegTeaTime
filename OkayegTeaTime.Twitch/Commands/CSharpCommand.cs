using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using HLE;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.CSharp)]
public readonly unsafe ref struct CSharpCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public CSharpCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string code = ChatMessage.Message[(messageExtension.Split[0].Length + 1)..];
            string result = GetProgramOutput(code);
            Response->Append(ChatMessage.Username, Messages.CommaSpace, result);
        }
    }

    private static string GetProgramOutput(string input)
    {
        try
        {
            string codeBlock = HttpUtility.HtmlEncode(ResourceController.CSharpTemplate.Replace("{code}", input));
            HttpPost request = new("https://dotnetfiddle.net/home/run", new[]
            {
                ("CodeBlock", codeBlock),
                ("Compiler", "Net7"),
                ("Language", "CSharp"),
                ("MvcViewEngine", "Razor"),
                ("NuGetPackageVersionIds", AppSettings.HleNugetVersionId),
                ("OriginalCodeBlock", codeBlock),
                ("OriginalFiddleId", "CsConsCore"),
                ("ProjectType", "Console"),
                ("TimeOffset", "1"),
                ("UseResultCache", "false")
            });
            if (request.Result is null)
            {
                throw new ArgumentNullException(nameof(request.Result));
            }

            JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
            string output = json.GetProperty("ConsoleOutput").GetString()!.NewLinesToSpaces();
            if (string.IsNullOrWhiteSpace(output))
            {
                return "executed successfully";
            }

            if (output.Length <= 450)
            {
                return output;
            }

            StringBuilder outputBuilder = stackalloc char[500];
            ReadOnlySpan<char> outputSpan = output;
            outputBuilder.Append(outputSpan[..450], "...");
            return outputBuilder.ToString();
        }
        catch (Exception ex)
        {
            if (input.Contains('\''))
            {
                return $"for some reason the API doesn't like the char \"'\". Please try to avoid it. {Emoji.SlightlySmilingFace}";
            }

            DbController.LogException(ex);
            return "an unexpected error occurred. The API might not be available.";
        }
    }
}
