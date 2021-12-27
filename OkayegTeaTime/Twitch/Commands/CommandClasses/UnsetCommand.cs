using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class UnsetCommand : Command
{
    public UnsetCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var prefixPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sprefix");
        if (prefixPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetPrefix(ChatMessage));
            return;
        }

        var reminderPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sreminder\s\d+");
        if (reminderPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetReminder(ChatMessage));
            return;
        }

        var emotePattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semote");
        if (emotePattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetEmoteInFront(ChatMessage));
            return;
        }

        var nukePattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\snuke\s\d+");
        if (nukePattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetNuke(ChatMessage));
        }
    }
}
