using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SubCommand : Command
{
    public SubCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        /*
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semotes?\s#?\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!ChatMessage.IsBroadcaster && !ChatMessage.IsModerator)
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
                return;
            }

            string subChannel = ChatMessage.LowerSplit[2].Remove("#");
            if (!TwitchApi.DoesUserExist(subChannel))
            {
                Response = $"{ChatMessage.Username}, the channel #{subChannel} doesn't exist";
                return;
            }

            TwitchBot.SubEmoteNotificator?.AddChannel(subChannel);
            DbController.AddSubEmoteSub(ChatMessage.ChannelId, subChannel);
            Response = $"{ChatMessage.Username}, channel #{ChatMessage.Channel} has subscribed to sub emote notifications for channel #{subChannel}";
            return;
        }
        */

        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semotes?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!ChatMessage.IsBroadcaster && !ChatMessage.IsModerator)
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
                return;
            }

            TwitchBot.EmoteManagementNotificator?.AddChannel(ChatMessage.Channel.Name);
            ChatMessage.Channel.IsEmoteSub = true;
            Response = $"{ChatMessage.Username}, channel #{ChatMessage.Channel} has subscribed to third party emote notifications";
        }
    }
}
