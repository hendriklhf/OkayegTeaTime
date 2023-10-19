using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Kotlin, typeof(KotlinCommand))]
public readonly struct KotlinCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<KotlinCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

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

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out KotlinCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string code = ChatMessage.Message[(messageExtension.Split[0].Length + 1)..];
            string result = await GetProgramOutputAsync(code);
            Response.Append(ChatMessage.Username, ", ", result);
        }
    }

    private static async ValueTask<string> GetProgramOutputAsync(string input)
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
            using StringContent content = new(JsonSerializer.Serialize(contentObj), Encoding.UTF8, "application/json");
            using HttpClient httpClient = new();
            using HttpResponseMessage response = await httpClient.PostAsync("https://api.kotlinlang.org/api/1.7.20/compiler/run", content);

            string responseString = await response.Content.ReadAsStringAsync();
            JsonElement json = JsonSerializer.Deserialize<JsonElement>(responseString);
            JsonElement errors = json.GetProperty("errors").GetProperty(_kotlinFileName);
            int errorLength = errors.GetArrayLength();
            if (errorLength > 0)
            {
                using PooledList<string> errorTexts = new();
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

                return string.Join(", ", errorTexts);
            }

            string? result = json.GetProperty("text").GetString();
            if (result is null)
            {
                return Messages.ApiError;
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                return "executed successfully";
            }

            // TODO: change this length check to the one used in CSharpCommand
            ReadOnlyMemory<char> resultSpan = result.AsMemory();
            resultSpan = resultSpan[_outStreamLabelLength..^(_outStreamLabelLength + 1)];
            if (resultSpan.Length <= 450)
            {
                return result[_outStreamLabelLength..^(_outStreamLabelLength + 1)].NewLinesToSpaces();
            }

            return resultSpan.Span.ToString().NewLinesToSpaces() + "...";
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return Messages.ApiError;
        }
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(KotlinCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is KotlinCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(KotlinCommand left, KotlinCommand right) => left.Equals(right);

    public static bool operator !=(KotlinCommand left, KotlinCommand right) => !left.Equals(right);
}
