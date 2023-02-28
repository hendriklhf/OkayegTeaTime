using System;
using System.Collections.Frozen;
using System.Text.Json;
using HLE;
using HLE.Memory;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Massping)]
public readonly unsafe ref struct MasspingCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    // ReSharper disable once NotAccessedField.Local
    private readonly string? _prefix;
    // ReSharper disable once NotAccessedField.Local
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

    public MasspingCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        if (_disabledChannels.Contains(ChatMessage.ChannelId))
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ThisCommandIsDisabledInThisChannel);
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator || !messageExtension.IsBroadcaster)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        string channelEmote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? AppSettings.DefaultEmote;
        ReadOnlySpan<char> emote = messageExtension.Split.Length > 1 ? messageExtension.Split[1] : channelEmote;
        using PoolBufferWriter<string> chatters = new(50, 50);
        if (ChatMessage.Channel != AppSettings.OfflineChatChannel)
        {
            GetChatters(ChatMessage.Channel, chatters);
            if (chatters.Length == 0)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.AnErrorOccurredOrThereAreNoChattersInThisChannel);
                return;
            }
        }
        else
        {
            Response->Append("OkayegTeaTime", StringHelper.Whitespace, emote, StringHelper.Whitespace);
            int emotesLength = AppSettings.OfflineChatEmotes.Length;
            AppSettings.OfflineChatEmotes.CopyTo(chatters.GetSpan(emotesLength));
            chatters.Advance(emotesLength);
        }

        Span<char> separator = stackalloc char[emote.Length + 2];
        separator[0] = ' ';
        separator[^1] = ' ';
        emote.CopyTo(separator[1..]);

        Span<char> joinBuffer = stackalloc char[500];
        int joinBufferLength = StringHelper.Join(chatters.WrittenSpan, separator, joinBuffer);
        Response->Append(joinBuffer[..joinBufferLength]);
    }

    private static void GetChatters(string channel, PoolBufferWriter<string> chatters)
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
            Span<string> channelDestination = chatters.GetSpan(chatterLength);
            for (int i = 0; i < chatterLength; i++)
            {
                channelDestination[i] = chatterList[i].GetString()!;
            }

            chatters.Advance(chatterLength);
        }
    }
}
