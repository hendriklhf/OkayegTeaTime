using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class ChattersCommand : Command
{
    public ChattersCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendChattersCount(ChatMessage));
        }
    }
}
