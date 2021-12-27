using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class RafkCommand : Command
{
    public RafkCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        TwitchBot.Send(ChatMessage.Channel, BotActions.SendResumingAfkStatus(ChatMessage));
    }
}
