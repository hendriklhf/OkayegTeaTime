using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class FuckCommand : Command
{
    public FuckCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+(\s\S+)?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFuck(ChatMessage));
        }
    }
}
