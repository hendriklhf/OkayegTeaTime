using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class UnsetCommand : Command
{
    public UnsetCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex prefixPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sprefix");
        if (prefixPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetPrefix(ChatMessage));
            return;
        }

        Regex reminderPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sreminder\s\d+");
        if (reminderPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetReminder(ChatMessage));
            return;
        }

        Regex emotePattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semote");
        if (emotePattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetEmoteInFront(ChatMessage));
            return;
        }
    }
}
