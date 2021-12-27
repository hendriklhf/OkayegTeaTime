using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class CountCommand : Command
{
    public CountCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var countForChannelPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s#\w+");
        if (countForChannelPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesChannelCount(ChatMessage));
            return;
        }

        var countForUserPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+");
        if (countForUserPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesUserCount(ChatMessage));
            return;
        }

        var totalMessagesPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix);
        if (totalMessagesPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesCount(ChatMessage));
        }
    }
}
