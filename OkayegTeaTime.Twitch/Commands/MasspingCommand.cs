using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly struct MasspingCommand : IChatCommand<MasspingCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public MasspingCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out MasspingCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        if (ChatMessage.Channel != AppSettings.OfflineChatChannel)
        {
            return ValueTask.CompletedTask;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOrTheBroadcaster);
            return ValueTask.CompletedTask;
        }

        string channelEmote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? AppSettings.DefaultEmote;
        ReadOnlySpan<char> emote = messageExtension.Split.Length > 1 ? messageExtension.Split[1].Span : channelEmote;
        using PoolBufferList<string> chatters = new(50, 50);
        Response.Append("OkayegTeaTime", " ", emote, " ");
        chatters.AddRange(AppSettings.OfflineChatEmotes);

        Span<char> separator = stackalloc char[emote.Length + 2];
        separator[0] = ' ';
        separator[^1] = ' ';
        emote.CopyTo(separator[1..]);

        int joinLength = StringHelper.Join(chatters.AsSpan(), separator, Response.FreeBufferSpan);
        Response.Advance(joinLength);
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
