using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HLE.Emojis;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.CSharp)]
public readonly unsafe ref struct CSharpCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public CSharpCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string code = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            string result = GetProgramOutput(code);
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, result);
        }
    }

    private static string GetProgramOutput(string input)
    {
        try
        {
            string codeBlock = HttpUtility.HtmlEncode(ResourceController.CSharpTemplate.Replace("{code}", input));
            using HttpClient httpClient = new();
            Task<HttpResponseMessage> postTask = httpClient.PostAsync("https://dotnetfiddle.net/home/run", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
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
            postTask.Wait();
            using HttpResponseMessage response = postTask.Result;
            Task<string> readContentTask = response.Content.ReadAsStringAsync();
            readContentTask.Wait();
            string result = readContentTask.Result;

            JsonElement json = JsonSerializer.Deserialize<JsonElement>(result);
            string output = json.GetProperty("ConsoleOutput").GetString()!.NewLinesToSpaces();
            return !string.IsNullOrWhiteSpace(output) ? output.Length > 450 ? $"{output[..450]}..." : output : "executed successfully";
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
