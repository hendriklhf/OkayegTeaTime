using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class CheckCommand : Command
{
    public CheckCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\safk\s\w+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckAfk(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sreminder\s\d+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckReminder(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\smessages?\s\d+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckMessage(ChatMessage));
        }
    }
}
