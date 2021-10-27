using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class UnsetCommand : Command
{
    public UnsetCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sprefix")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetPrefix(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sreminder\s\d+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetReminder(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semote")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetEmoteInFront(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\snuke\s\d+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetNuke(ChatMessage));
        }
    }
}
