using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Massping)]
public class MasspingCommand : Command
{
    private readonly long[] _disabledChannels =
    {
        35933008
    };

    private readonly string[] _chatRoles =
    {
        "broadcaster",
        "vips",
        "moderators",
        "staff",
        "admins",
        "global_mods",
        "viewers"
    };

    public MasspingCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (_disabledChannels.Contains(ChatMessage.ChannelId))
        {
            Response = $"{ChatMessage.Username}, this command is disabled in this channel";
            return;
        }

        if (!ChatMessage.IsModerator && !ChatMessage.IsBroadcaster)
        {
            Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
            return;
        }

        string channelEmote = DbControl.Channels[ChatMessage.ChannelId]?.Emote ?? AppSettings.DefaultEmote;
        string emote = ChatMessage.Split.Length > 1 ? ChatMessage.Split[1] : channelEmote;
        string[] chatters;
        if (ChatMessage.Channel != AppSettings.OfflineChatChannel)
        {
            chatters = GetChatters(ChatMessage.Channel).Select(c => c.Username).ToArray();
            if (chatters.Length == 0)
            {
                Response = string.Empty;
                return;
            }
        }
        else
        {
            Response = $"OkayegTeaTime {emote} ";
            chatters = AppSettings.OfflineChatEmotes;
        }

        Response += string.Join($" {emote} ", chatters);
    }

    private IEnumerable<Chatter> GetChatters(string channel)
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

        return result;
    }
}
