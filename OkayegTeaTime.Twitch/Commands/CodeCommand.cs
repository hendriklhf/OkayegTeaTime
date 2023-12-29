using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Code, typeof(CodeCommand))]
public readonly struct CodeCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<CodeCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private static readonly StringArray s_codeFiles = new(ResourceController.CodeFiles.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).AsSpan());

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out CodeCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask HandleAsync()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Regex? fileRegex;
            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                ReadOnlyMemory<char> filePattern = ChatMessage.Message.AsMemory((messageExtension.Split[0].Length + 1)..);
                fileRegex = RegexPool.Shared.GetOrAdd(filePattern.Span, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            }
            catch (ArgumentException)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.TheGivenPatternIsInvalid);
                return ValueTask.CompletedTask;
            }

            using PooledList<string> matchingFiles = [];
            GetMatchingFiles(s_codeFiles, matchingFiles, fileRegex);

            Response.Append(ChatMessage.Username, ", ");
            switch (matchingFiles.Count)
            {
                case 0:
                    Response.Append(Messages.YourPatternMatchedNoSourceCodeFiles);
                    break;
                case 1:
                    Response.Append(GlobalSettings.Settings.RepositoryUrl, "/blob/master/", matchingFiles[0]);
                    break;
                case <= 5:
                    Response.Append("your pattern matched ");
                    Response.Append(matchingFiles.Count);
                    Response.Append(" files: ");

                    int joinLength = StringHelpers.Join(", ", matchingFiles.AsSpan(), Response.FreeBufferSpan);
                    Response.Advance(joinLength);

                    Response.Append(". Please specify.");
                    break;
                default:
                    Response.Append("your pattern matched too many (");
                    Response.Append(matchingFiles.Count);
                    Response.Append(") files. Please specify.");
                    break;
            }
        }

        return ValueTask.CompletedTask;
    }

    private static void GetMatchingFiles(StringArray codeFiles, PooledList<string> matchingFiles, Regex fileRegex)
    {
        for (int i = 0; i < codeFiles.Length; i++)
        {
            ReadOnlySpan<char> file = codeFiles.GetChars(i);
            if (fileRegex.IsMatch(file))
            {
                matchingFiles.Add(codeFiles[i]);
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
