using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class TuckCommand : Command
{
    public TuckCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+");
        if (!pattern.IsMatch(ChatMessage.Message))
            return;

        TwitchBot.Send(ChatMessage.Channel, BotActions.SendTuckedToBed(ChatMessage));
    }
}
