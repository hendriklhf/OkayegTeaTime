using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Kotlin)]
public sealed class KotlinCommand : Command
{
    private readonly HttpClient _httpClient = new();

    private const string _kotlinFileName = "File.kt";
    /// <summary>
    /// Length of "fun main() { "
    /// </summary>
    private const byte _mainFunLength = 13;
    /// <summary>
    /// Length of "&lt;outStream&gt;"
    /// </summary>
    private const byte _outStreamLabelLength = 11;
    private const string _errorSeverity = "ERROR";

    public KotlinCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
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

    private async Task<string> GetProgramOutput(string input)
    {
        try
        {
            string code = ResourceController.KotlinTemplate.Replace("{code}", input);
            object contentObj = new
            {
                args = string.Empty,
                files = new[]
                {
                    new
                    {
                        name = _kotlinFileName,
                        publicId = string.Empty,
                        text = code[..^2]
                    }
                },
                confType = "java"
            };
            StringContent content = new(JsonSerializer.Serialize(contentObj), Encoding.UTF8, "application/json");
            using HttpResponseMessage responseMessage = await _httpClient.PostAsync("https://api.kotlinlang.org/api/1.7.20/compiler/run", content);
            string response = await responseMessage.Content.ReadAsStringAsync();
            JsonElement json = JsonSerializer.Deserialize<JsonElement>(response);
            JsonElement errors = json.GetProperty("errors").GetProperty(_kotlinFileName);
            int errorLength = errors.GetArrayLength();
            if (errorLength > 0)
            {
                List<string> errorTexts = new();
                for (int i = 0; i < errors.GetArrayLength(); i++)
                {
                    JsonElement error = errors[i];
                    string severity = error.GetProperty("severity").GetString()!;
                    if (severity != _errorSeverity)
                    {
                        continue;
                    }

                    string message = error.GetProperty("message").GetString()!;
                    int start = error.GetProperty("interval").GetProperty("start").GetProperty("ch").GetInt32();
                    errorTexts.Add($"{severity} at ch{start - _mainFunLength}: {message}");
                }

                return string.Join(", ", errorTexts);
            }

            string? result = json.GetProperty("text").GetString();
            if (result is null)
            {
                return "api error";
            }

            result = result[_outStreamLabelLength..^(_outStreamLabelLength + 1)];
            return string.IsNullOrWhiteSpace(result) ? "executed successfully" : (result.Length > 450 ? $"{result[..450]}..." : result).NewLinesToSpaces();
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return "api error";
        }
    }
}
