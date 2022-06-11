using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Models.Enums;

namespace OkayegTeaTime.Twitch.Commands;

public class MasspingCommand : Command
{
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
        if ((ChatMessage.IsModerator || ChatMessage.IsBroadcaster) && ChatMessage.Channel != "moondye7")
        {
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
        else
        {
            Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
        }
    }

    private IEnumerable<Chatter> GetChatters(string channel)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel}/chatters");
        List<Chatter> result = new();
        if (!request.IsValidJsonData)
        {
            return result;
        }

        JsonElement chatters = request.Data.GetProperty("chatters");
        byte pIdx = 0;
        foreach (string p in _chatRoles)
        {
            JsonElement chatterList = chatters.GetProperty(p);
            for (int i = 0; i < chatterList.GetArrayLength(); i++)
            {
                result.Add(new(chatterList[i].GetString()!, (ChatRole)pIdx++));
            }
        }
        return result;
    }
}
