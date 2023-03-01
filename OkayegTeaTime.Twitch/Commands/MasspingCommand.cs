using System;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using HLE.Collections;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Massping)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly ref struct MasspingCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly FrozenSet<long> _disabledChannels = new long[]
    {
        35933008,
        93156665
    }.ToFrozenSet();

    private static readonly string[] _chatRoles =
    {
        "broadcaster",
        "vips",
        "moderators",
        "staff",
        "admins",
        "global_mods",
        "viewers"
    };

    public MasspingCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        if (_disabledChannels.Contains(ChatMessage.ChannelId))
        {
            _response.Append(ChatMessage.Username, ", ", Messages.ThisCommandIsDisabledInThisChannel);
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
        if (ChatMessage.Channel != AppSettings.OfflineChatChannel)
        {
            GetChatters(ChatMessage.Channel, chatters);
            if (chatters.Count == 0)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.AnErrorOccurredOrThereAreNoChattersInThisChannel);
                return;
            }
        }
        else
        {
            _response.Append("OkayegTeaTime", " ", emote, " ");
            chatters.AddRange(AppSettings.OfflineChatEmotes);
        }

        Span<char> separator = stackalloc char[emote.Length + 2];
        separator[0] = ' ';
        separator[^1] = ' ';
        emote.CopyTo(separator[1..]);

        Span<char> joinBuffer = stackalloc char[500];
        int joinBufferLength = StringHelper.Join(chatters.AsSpan(), separator, joinBuffer);
        _response.Append(joinBuffer[..joinBufferLength]);
    }

    private static void GetChatters(string channel, PoolBufferList<string> chatters)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel}/chatters");
        if (request.Result is null)
        {
            return;
        }

        JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
        JsonElement jsonChatters = json.GetProperty("chatters");
        foreach (string role in _chatRoles)
        {
            JsonElement chatterList = jsonChatters.GetProperty(role);
            int chatterLength = chatterList.GetArrayLength();
            for (int i = 0; i < chatterLength; i++)
            {
                chatters.Add(chatterList[i].GetString()!);
            }
        }
    }
}
