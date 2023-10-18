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
    public PooledStringBuilder Response { get; } = new(AppSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private static StringArray? _codeFiles;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out CodeCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask Handle()
    {
        _codeFiles ??= new(ResourceController.CodeFiles.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).AsSpan());

        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Regex? filePattern;
            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                filePattern = new(ChatMessage.Message[(messageExtension.Split[0].Length + 1)..], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            }
            catch (ArgumentException)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.TheGivenPatternIsInvalid);
                return ValueTask.CompletedTask;
            }

            using PooledList<string> matchingFiles = new(5);
            GetMatchingFiles(_codeFiles, matchingFiles, filePattern);
            Response.Append(ChatMessage.Username, ", ");
            switch (matchingFiles.Count)
            {
                case 0:
                    Response.Append(Messages.YourPatternMatchedNoSourceCodeFiles);
                    break;
                case 1:
                    Response.Append(AppSettings.RepositoryUrl, "/blob/master/", matchingFiles[0]);
                    break;
                case <= 5:
                    Response.Append("your pattern matched ");
                    Response.Append(matchingFiles.Count);
                    Response.Append(" files: ");

                    int joinLength = StringHelper.Join(matchingFiles.AsSpan(), ", ", Response.FreeBufferSpan);
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

    private static void GetMatchingFiles(StringArray codeFiles, PooledList<string> matchingFiles, Regex filePattern)
    {
        for (int i = 0; i < codeFiles.Length; i++)
        {
            ReadOnlySpan<char> file = codeFiles.GetChars(i);
            if (filePattern.IsMatch(file))
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
