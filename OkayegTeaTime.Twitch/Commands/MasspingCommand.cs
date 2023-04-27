using System;
using System.Diagnostics.CodeAnalysis;
using HLE.Collections;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly ref struct MasspingCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref PoolBufferStringBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public MasspingCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref PoolBufferStringBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        if (ChatMessage.Channel != AppSettings.OfflineChatChannel)
        {
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        string channelEmote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? AppSettings.DefaultEmote;
        ReadOnlySpan<char> emote = messageExtension.Split.Length > 1 ? messageExtension.Split[1] : channelEmote;
        using PoolBufferList<string> chatters = new(50, 50);
        _response.Append("OkayegTeaTime", " ", emote, " ");
        chatters.AddRange(AppSettings.OfflineChatEmotes);

        Span<char> separator = stackalloc char[emote.Length + 2];
        separator[0] = ' ';
        separator[^1] = ' ';
        emote.CopyTo(separator[1..]);

        int joinLength = StringHelper.Join(chatters.AsSpan(), separator, _response.FreeBufferSpan);
        _response.Advance(joinLength);
    }
}
