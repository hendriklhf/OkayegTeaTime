using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class TuckCommand : Command
{
    public TuckCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+");
        if (!pattern.IsMatch(ChatMessage.Message))
        {
            return;
        }

        TwitchBot.Send(ChatMessage.Channel, BotActions.SendTuckedToBed(ChatMessage));
    }
}
