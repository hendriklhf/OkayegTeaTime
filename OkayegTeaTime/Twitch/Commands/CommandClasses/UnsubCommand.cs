using System.Text.RegularExpressions;
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
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semotes?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsBroadcaster || ChatMessage.IsModerator)
            {
                TwitchBot.EmoteManagementNotificator?.RemoveChannel(ChatMessage.Channel.Name);
                ChatMessage.Channel.IsEmoteSub = false;
                Response += $"channel #{ChatMessage.Channel} has unsubscribed from the emote notifications";
            }
            else
            {
                Response += PredefinedMessages.NoModOrBroadcasterMessage;
            }
        }
    }
}
