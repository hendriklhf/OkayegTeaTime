using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<MasspingCommand>(CommandType.Massping)]
public readonly struct MasspingCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<MasspingCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out MasspingCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    [SuppressMessage("Roslynator", "RCS1229:Use async/await when necessary", Justification = "ChatMessageExtension can be disposed before the Task is awaited")]
    public ValueTask HandleAsync()
    {
        if (GlobalSettings.Settings.OfflineChat is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.TheCommandHasNotBeenConfiguredByTheBotOwner}");
            return ValueTask.CompletedTask;
        }

        if (ChatMessage.Channel != GlobalSettings.Settings.OfflineChat.Channel)
        {
            return ValueTask.CompletedTask;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.YouArentAModeratorOrTheBroadcaster}");
            return ValueTask.CompletedTask;
        }

        string channelEmote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? GlobalSettings.DefaultEmote;
        ReadOnlySpan<char> emote = messageExtension.Split.Length > 1 ? messageExtension.Split[1].Span : channelEmote;
        using PooledList<string> chatters = [];
        Response.Append($"OkayegTeaTime {emote} ");
        chatters.AddRange(GlobalSettings.Settings.OfflineChat!.Emotes.AsSpan());

        Span<char> separator = stackalloc char[emote.Length + 2];
        separator[0] = ' ';
        separator[^1] = ' ';
        emote.CopyTo(separator[1..]);

        int joinLength = StringHelpers.Join(separator, chatters.AsSpan(), Response.FreeBufferSpan);
        Response.Advance(joinLength);
        return ValueTask.CompletedTask;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(MasspingCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is MasspingCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(MasspingCommand left, MasspingCommand right) => left.Equals(right);

    public static bool operator !=(MasspingCommand left, MasspingCommand right) => !left.Equals(right);
}
