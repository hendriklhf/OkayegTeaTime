using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using HLE;
using HLE.Http;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Massping)]
public readonly unsafe ref struct MasspingCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    private readonly TwitchBot _twitchBot;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string? _prefix;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string _alias;

    private static readonly long[] _disabledChannels =
    {
        35933008
    };

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

    public MasspingCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
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
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.ThisCommandIsDisabledInThisChannel);
            return;
        }

        if (ChatMessage is { IsModerator: false, IsBroadcaster: false })
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentAModeratorOfTheBot);
            return;
        }

        string channelEmote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? AppSettings.DefaultEmote;
        string emote = ChatMessage.Split.Length > 1 ? ChatMessage.Split[1] : channelEmote;
        string[] chatters;
        if (ChatMessage.Channel != AppSettings.OfflineChatChannel)
        {
            chatters = GetChatters(ChatMessage.Channel).Select(c => c.Username).ToArray();
            if (chatters.Length == 0)
            {
                return;
            }
        }
        else
        {
            Response->Append("OkayegTeaTime", StringHelper.Whitespace, emote, StringHelper.Whitespace);
            chatters = AppSettings.OfflineChatEmotes;
        }

        ReadOnlySpan<char> users = string.Join($" {emote} ", chatters);
        if (users.Length > 2000)
        {
            Response->Append(users[..1997], "...");
            return;
        }

        Response->Append(users);
    }

    private static Chatter[] GetChatters(string channel)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel}/chatters");
        if (!request.IsValidJsonData)
        {
            return Array.Empty<Chatter>();
        }

        List<Chatter> result = new();
        JsonElement chatters = request.Data.GetProperty("chatters");
        byte roleIdx = 0;
        foreach (string role in _chatRoles)
        {
            JsonElement chatterList = chatters.GetProperty(role);
            for (int i = 0; i < chatterList.GetArrayLength(); i++)
            {
                result.Add(new(chatterList[i].GetString()!, (ChatRole)roleIdx++));
            }
        }

        return result.ToArray();
    }
}
