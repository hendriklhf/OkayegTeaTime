using OkayegTeaTime.Database;
using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public class MasspingCommand : Command
{
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
                chatters = HttpRequest.GetChatters(ChatMessage.Channel).Select(c => c.Username).ToArray();
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
}
