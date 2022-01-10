using System.Text.RegularExpressions;
using HLE.Strings;
using OkayegTeaTime.Database;
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
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                ChatMessage.Channel.Prefix = null;
                Response += "the prefix has been unset";
            }
            else
            {
                Response += PredefinedMessages.NoModOrBroadcasterMessage;
            }
            return;
        }

        Regex reminderPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sreminder\s\d+");
        if (reminderPattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            int reminderId = ChatMessage.Split[2].ToInt();
            bool removed = DbController.RemoveReminder(ChatMessage, reminderId);
            if (removed)
            {
                Response += "the reminder has been unset";
            }
            else
            {
                Response += "the reminder couldn't be unset";
            }
            return;
        }

        Regex emotePattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semote");
        if (emotePattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                ChatMessage.Channel.Emote = AppSettings.DefaultEmote;
                Response += "unset emote";
            }
            else
            {
                Response += PredefinedMessages.NoModOrBroadcasterMessage;
            }
            return;
        }
    }
}
