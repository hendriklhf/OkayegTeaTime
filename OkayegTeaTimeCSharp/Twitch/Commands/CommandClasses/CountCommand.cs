using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class CountCommand : Command
{
    public CountCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s#\w+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesChannelCount(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesUserCount(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix)))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesCount(ChatMessage));
        }
    }
}
