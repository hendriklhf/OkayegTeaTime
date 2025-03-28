using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<TuckCommand>(CommandType.Tuck)]
public readonly struct TuckCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<TuckCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out TuckCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask HandleAsync()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\w+(\s\S+)?");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            ReadOnlySpan<char> target = messageExtension.LowerSplit[1].Span;
            Response.Append($"{Emoji.PointRight} {Emoji.Bed} {ChatMessage.Username}");

            Response.Append($" tucked {target} to bed");
            ReadOnlySpan<char> emote = messageExtension.LowerSplit.Length > 2 ? messageExtension.Split[2].Span : [];
            if (emote.Length != 0)
            {
                Response.Append(' ');
                Response.Append(emote);
            }
        }

        return ValueTask.CompletedTask;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(TuckCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is TuckCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(TuckCommand left, TuckCommand right) => left.Equals(right);

    public static bool operator !=(TuckCommand left, TuckCommand right) => !left.Equals(right);
}
