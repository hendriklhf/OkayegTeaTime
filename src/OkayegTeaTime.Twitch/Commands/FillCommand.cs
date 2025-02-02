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

[HandledCommand<FillCommand>(CommandType.Fill)]
public readonly struct FillCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<FillCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out FillCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    [SuppressMessage("Roslynator", "RCS1229:Use async/await when necessary")]
    public ValueTask HandleAsync()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string emote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? GlobalSettings.DefaultEmote;
            int maxLength = GlobalSettings.MaxMessageLength - (emote.Length + 1);
            ReadOnlySpan<ReadOnlyMemory<char>> messageParts = messageExtension.Split.AsSpan()[1..];
            if (messageParts.Length == 1)
            {
                HandleSingleMessagePart(messageParts[0].Span, maxLength);
                return ValueTask.CompletedTask;
            }

            ReadOnlyMemory<char> nextMessagePart = Random.Shared.GetItem(messageParts);
            for (int currentMessageLength = 0; currentMessageLength + nextMessagePart.Length + 1 < maxLength; currentMessageLength += nextMessagePart.Length + 1)
            {
                Response.Append(nextMessagePart.Span);
                Response.Append(' ');
                nextMessagePart = Random.Shared.GetItem(messageParts);
            }
        }

        return ValueTask.CompletedTask;
    }

    private void HandleSingleMessagePart(ReadOnlySpan<char> item, int maxLength)
    {
        if (Emoji.IsEmoji(item))
        {
            int count = maxLength / item.Length;
            for (int i = 0; i < count; i++)
            {
                Response.Append(item);
            }

            return;
        }

        int length = item.Length;
        Response.Append(item);
        while (length + item.Length + 1 < maxLength)
        {
            Response.Append(' ');
            Response.Append(item);
            length += item.Length + 1;
        }
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(FillCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is FillCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(FillCommand left, FillCommand right) => left.Equals(right);

    public static bool operator !=(FillCommand left, FillCommand right) => !left.Equals(right);
}
