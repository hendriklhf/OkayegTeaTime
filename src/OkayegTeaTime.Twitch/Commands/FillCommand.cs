using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Fill, typeof(FillCommand))]
public readonly struct FillCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<FillCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out FillCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask HandleAsync()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string emote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? GlobalSettings.DefaultEmote;
            int maxLength = GlobalSettings.MaxMessageLength - (emote.Length + 1);
            ReadOnlyMemory<char> nextMessagePart = messageExtension.Split[Random.Shared.Next(1, messageExtension.Split.Length)];
            for (int currentMessageLength = 0; currentMessageLength + nextMessagePart.Length + 1 < maxLength; currentMessageLength += nextMessagePart.Length + 1)
            {
                Response.Append(nextMessagePart.Span);
                Response.Append(' ');
                nextMessagePart = messageExtension.Split[Random.Shared.Next(1, messageExtension.Split.Length)];
            }
        }

        return ValueTask.CompletedTask;
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
