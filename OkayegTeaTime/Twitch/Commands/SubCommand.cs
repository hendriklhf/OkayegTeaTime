using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public class SubCommand : Command
{
    public SubCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\semotes?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!ChatMessage.IsBroadcaster && !ChatMessage.IsModerator)
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
                return;
            }

            _twitchBot.EmoteManagementNotificator?.AddChannel(ChatMessage.Channel);
            Channel? channel = DbControl.Channels[ChatMessage.ChannelId];
            if (channel is null)
            {
                Response = $"{ChatMessage.Username}, an error occurred while trying to sub to the emote notifications";
                return;
            }

            channel.IsEmoteNotificationSub = true;
            Response = $"{ChatMessage.Username}, channel #{ChatMessage.Channel} has subscribed to third party emote notifications";
        }
    }
}
