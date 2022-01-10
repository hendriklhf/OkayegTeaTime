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
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semotes?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (ChatMessage.IsBroadcaster || ChatMessage.IsModerator)
            {
                TwitchBot.EmoteManagementNotificator?.AddChannel(ChatMessage.Channel.Name);
                ChatMessage.Channel.IsEmoteSub = true;
                Response = $"{ChatMessage.Username}, channel #{ChatMessage.Channel} has subscribed to the emote notifications";
            }
            else
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
            }
        }
    }
}
