using HLE.Strings;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class RandomWordCommand : Command
{
    public RandomWordCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var countPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\d+");
        if (countPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendRandomWords(ChatMessage, ChatMessage.Split[1].ToInt()));
            return;
        }

        TwitchBot.Send(ChatMessage.Channel, BotActions.SendRandomWords(ChatMessage));
    }
}
