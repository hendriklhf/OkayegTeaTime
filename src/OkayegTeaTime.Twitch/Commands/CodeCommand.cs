using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<CodeCommand>(CommandType.Code)]
public readonly struct CodeCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<CodeCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private static readonly string[] s_codeFiles = ResourceController.CodeFiles.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out CodeCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask HandleAsync()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            Regex? fileRegex;
            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                ReadOnlyMemory<char> filePattern = ChatMessage.Message.AsMemory()[(messageExtension.Split[0].Length + 1)..];
                fileRegex = RegexPool.Shared.GetOrAdd(filePattern.Span, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            }
            catch (ArgumentException)
            {
                Response.Append($"{ChatMessage.Username}, {Texts.TheGivenPatternIsInvalid}");
                return ValueTask.CompletedTask;
            }

            using PooledList<string> matchingFiles = [];
            GetMatchingFiles(matchingFiles, fileRegex);

            Response.Append($"{ChatMessage.Username}, ");
            switch (matchingFiles.Count)
            {
                case 0:
                    Response.Append(Texts.YourPatternMatchedNoSourceCodeFiles);
                    break;
                case 1:
                    Response.Append($"{GlobalSettings.Settings.RepositoryUrl}/blob/master/{matchingFiles[0]}");
                    break;
                case <= 5:
                    Response.Append($"your pattern matched {matchingFiles.Count} files: ");

                    int joinLength = StringHelpers.Join(", ", matchingFiles.AsSpan(), Response.FreeBufferSpan);
                    Response.Advance(joinLength);

                    Response.Append(". Please specify.");
                    break;
                default:
                    Response.Append($"your pattern matched too many ({matchingFiles.Count}) files. Please specify.");
                    break;
            }
        }

        return ValueTask.CompletedTask;
    }

    private static void GetMatchingFiles(PooledList<string> matchingFiles, Regex fileRegex)
    {
        string[] codeFiles = s_codeFiles;
        for (int i = 0; i < codeFiles.Length; i++)
        {
            string codeFile = codeFiles[i];
            if (fileRegex.IsMatch(codeFile))
            {
                matchingFiles.Add(codeFile);
            }
        }
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(CodeCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is CodeCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(CodeCommand left, CodeCommand right) => left.Equals(right);

    public static bool operator !=(CodeCommand left, CodeCommand right) => !left.Equals(right);
}
