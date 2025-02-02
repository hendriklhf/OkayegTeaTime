using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<PickCommand>(CommandType.Pick)]
public readonly struct PickCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<PickCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out PickCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    [SuppressMessage("Roslynator", "RCS1229:Use async/await when necessary", Justification = "ChatMessageExtension can be disposed before the Task is awaited")]
    public ValueTask HandleAsync()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (!pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            Response.Append($"{ChatMessage.Username}, {Texts.NoItemsProvided}");
            return ValueTask.CompletedTask;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        ReadOnlyMemory<char> randomPick = Random.Shared.GetItem(messageExtension.Split.AsSpan()[1..]);
        Response.Append($"{ChatMessage.Username}, {randomPick.Span}");
        return ValueTask.CompletedTask;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(PickCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is PickCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(PickCommand left, PickCommand right) => left.Equals(right);

    public static bool operator !=(PickCommand left, PickCommand right) => !left.Equals(right);
}
