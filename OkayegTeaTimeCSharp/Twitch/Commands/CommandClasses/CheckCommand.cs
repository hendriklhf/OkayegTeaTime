using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class CheckCommand : Command
{
    public CheckCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var afkPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\safk\s\w+");
        if (afkPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckAfk(ChatMessage));
            return;
        }

        var reminderPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sreminder\s\d+");
        if (reminderPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckReminder(ChatMessage));
            return;
        }

        var messagesPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\smessages?\s\d+");
        if (messagesPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckMessage(ChatMessage));
        }
    }
}
