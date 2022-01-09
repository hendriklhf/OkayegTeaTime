using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class CheckCommand : Command
{
    public CheckCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex afkPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\safk\s\w+");
        if (afkPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckAfk(ChatMessage));
            return;
        }

        Regex reminderPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sreminder\s\d+");
        if (reminderPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckReminder(ChatMessage));
            return;
        }
    }
}
