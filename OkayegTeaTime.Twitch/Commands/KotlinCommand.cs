using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
public readonly unsafe ref struct KotlinCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
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

    public KotlinCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
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

            result = result[_outStreamLabelLength..^(_outStreamLabelLength + 1)];
            return string.IsNullOrWhiteSpace(result) ? "executed successfully" : (result.Length > 450 ? $"{result[..450]}..." : result).NewLinesToSpaces();
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return PredefinedMessages.ApiError;
        }
    }
}
