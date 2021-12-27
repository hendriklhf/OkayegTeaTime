using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class PickCommand : Command
{
    public PickCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        TwitchBot.Send(ChatMessage.Channel, BotActions.SendPick(ChatMessage));
    }
}
