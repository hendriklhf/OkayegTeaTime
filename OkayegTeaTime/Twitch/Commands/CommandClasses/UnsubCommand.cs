using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class UnsubCommand : Command
{
    public UnsubCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\semotes?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsBroadcaster || ChatMessage.IsModerator)
            {
                _twitchBot.EmoteManagementNotificator?.RemoveChannel(ChatMessage.Channel);
                Channel? channel = DbControl.Channels[ChatMessage.ChannelId];
                if (channel is null)
                {
                    Response += "an error occurred while trying to unsub to emote notifications";
                    return;
                }

                channel.IsEmoteNotificationSub = false;
                Response += $"channel #{ChatMessage.Channel} has unsubscribed from third party emote notifications";
            }
            else
            {
                Response += PredefinedMessages.NoModOrBroadcasterMessage;
            }
        }
    }
}
