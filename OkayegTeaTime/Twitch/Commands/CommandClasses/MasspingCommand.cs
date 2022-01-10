using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class MasspingCommand : Command
{
    public MasspingCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster && ChatMessage.Channel.Name != "moondye7")
        {
            string emote = ChatMessage.Split.Length > 1 ? ChatMessage.Split[1] : ChatMessage.Channel.Emote;
            List<string> chatters;
            if (ChatMessage.Channel.Name != AppSettings.SecretOfflineChatChannel)
            {
                chatters = HttpRequest.GetChatters(ChatMessage.Channel.Name).Select(c => c.Username).ToList();
                if (chatters.Count == 0)
                {
                    Response = string.Empty;
                    return;
                }
            }
            else
            {
                Response = $"OkayegTeaTime {emote} ";
                chatters = AppSettings.SecretOfflineChatEmotes;
            }
            Response += string.Join($" {emote} ", chatters);
        }
        else
        {
            Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
        }
    }
}
