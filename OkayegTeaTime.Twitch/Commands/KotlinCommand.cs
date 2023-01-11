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
using StringBuilder = HLE.StringBuilder;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Kotlin)]
public readonly unsafe ref struct KotlinCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

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

    public KotlinCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
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
            string code = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            string result = GetProgramOutput(code);
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, result);
        }
    }

    private static string GetProgramOutput(string input)
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
                        text = code[..^2] //these two chars (\r\n) have to be removed, otherwise the api returns 502
                    }
                },
                confType = "java"
            };
            StringContent content = new(JsonSerializer.Serialize(contentObj), Encoding.UTF8, "application/json");
            using HttpClient httpClient = new();
            Task<HttpResponseMessage> postTask = httpClient.PostAsync("https://api.kotlinlang.org/api/1.7.20/compiler/run", content);
            postTask.Wait();
            using HttpResponseMessage responseMessage = postTask.Result;

            Task<string> readContentTask = responseMessage.Content.ReadAsStringAsync();
            readContentTask.Wait();
            string response = readContentTask.Result;
            JsonElement json = JsonSerializer.Deserialize<JsonElement>(response);
            JsonElement errors = json.GetProperty("errors").GetProperty(_kotlinFileName);
            int errorLength = errors.GetArrayLength();
            if (errorLength > 0)
            {
                List<string> errorTexts = new();
                for (int i = 0; i < errorLength; i++)
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

                return string.Join(PredefinedMessages.CommaSpace, errorTexts);
            }

            string? result = json.GetProperty("text").GetString();
            if (result is null)
            {
                return PredefinedMessages.ApiError;
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                return "executed successfully";
            }

            ReadOnlySpan<char> resultSpan = result;
            resultSpan = resultSpan[_outStreamLabelLength..^(_outStreamLabelLength + 1)];
            if (resultSpan.Length <= 450)
            {
                return result[_outStreamLabelLength..^(_outStreamLabelLength + 1)].NewLinesToSpaces();
            }

            StringBuilder resultBuilder = stackalloc char[500];
            resultBuilder.Append(resultSpan, "...");
            return resultBuilder.ToString().NewLinesToSpaces();
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return PredefinedMessages.ApiError;
        }
    }
}
