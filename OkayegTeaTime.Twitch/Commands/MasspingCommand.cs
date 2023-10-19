using System;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Massping, typeof(MasspingCommand))]
public readonly struct MasspingCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<MasspingCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out MasspingCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask HandleAsync()
    {
        if (GlobalSettings.Settings.OfflineChat is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.TheCommandHasNotBeenConfiguredByTheBotOwner);
            return ValueTask.CompletedTask;
        }

        if (ChatMessage.Channel != GlobalSettings.Settings.OfflineChat.Channel)
        {
            return ValueTask.CompletedTask;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOrTheBroadcaster);
            return ValueTask.CompletedTask;
        }

        string channelEmote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? GlobalSettings.DefaultEmote;
        ReadOnlySpan<char> emote = messageExtension.Split.Length > 1 ? messageExtension.Split[1].Span : channelEmote;
        using PooledList<string> chatters = new();
        Response.Append("OkayegTeaTime", " ", emote, " ");
        chatters.AddRange(GlobalSettings.Settings.OfflineChat!.Emotes.AsSpan());

        Span<char> separator = stackalloc char[emote.Length + 2];
        separator[0] = ' ';
        separator[^1] = ' ';
        emote.CopyTo(separator[1..]);

        int joinLength = StringHelper.Join(chatters.AsSpan(), separator, Response.FreeBufferSpan);
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
