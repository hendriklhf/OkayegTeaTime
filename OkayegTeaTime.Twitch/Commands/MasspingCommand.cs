using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using HLE;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Massping)]
public readonly unsafe ref struct MasspingCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string? _prefix;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string _alias;

    private static readonly long[] _disabledChannels =
    {
        35933008,
        93156665
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

    public MasspingCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
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

        if (ChatMessage is { IsModerator: false, IsBroadcaster: false })
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentAModeratorOfTheBot);
            return;
        }

        string channelEmote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? AppSettings.DefaultEmote;
        string emote = ChatMessage.Split.Length > 1 ? ChatMessage.Split[1] : channelEmote;
        string[] chatters;
        if (ChatMessage.Channel != AppSettings.OfflineChatChannel)
        {
            chatters = GetChatters(ChatMessage.Channel);
            if (chatters.Length == 0)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.AnErrorOccurredOrThereAreNoChattersInThisChannel);
                return;
            }
        }
        else
        {
            Response->Append("OkayegTeaTime", StringHelper.Whitespace, emote, StringHelper.Whitespace);
            chatters = AppSettings.OfflineChatEmotes;
        }

        ReadOnlySpan<char> users = string.Join($" {emote} ", chatters);
        Response->Append(users);
    }

    private static string[] GetChatters(string channel)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel}/chatters");
        if (request.Result is null)
        {
            return Array.Empty<string>();
        }

        JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
        List<string> result = new();
        JsonElement chatters = json.GetProperty("chatters");
        foreach (string role in _chatRoles)
        {
            JsonElement chatterList = chatters.GetProperty(role);
            int chatterLength = chatterList.GetArrayLength();
            for (int i = 0; i < chatterLength; i++)
            {
                result.Add(chatterList[i].GetString()!);
            }
        }

        return result.ToArray();
    }
}
