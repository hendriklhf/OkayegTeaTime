using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class FirstCommand : Command
{
    public FirstCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var firstForUserInChannelPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+\s#?\w+");
        if (firstForUserInChannelPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirstUserChannel(ChatMessage));
            return;
        }

        var firstInChannelPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s#\w+");
        if (firstInChannelPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirstChannel(ChatMessage));
            return;
        }

        var firstForUserPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+");
        if (firstForUserPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirstUser(ChatMessage));
            return;
        }

        var pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirst(ChatMessage));
        }
    }
}
