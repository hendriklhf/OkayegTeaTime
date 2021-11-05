using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class RandomWordCommand : Command
{
    public RandomWordCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\d+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendRandomWords(ChatMessage, ChatMessage.Split[1].ToInt()));
        }
        else
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendRandomWords(ChatMessage));
        }
    }
}
