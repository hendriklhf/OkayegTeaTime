using System.Text.RegularExpressions;
using HLE.Strings;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
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
        Regex prefixPattern = PatternCreator.Create(Alias, Prefix, @"\sprefix");
        if (prefixPattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                Channel? channel = DbControl.Channels[ChatMessage.ChannelId];
                if (channel is null)
                {
                    Response += "an error occurred while trying to set the prefix";
                    return;
                }

                channel.Prefix = null;
                Response += "the prefix has been unset";
            }
            else
            {
                Response += PredefinedMessages.NoModOrBroadcasterMessage;
            }

            return;
        }

        Regex reminderPattern = PatternCreator.Create(Alias, Prefix, @"\sreminder\s\d+(\s|$)");
        if (reminderPattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            int reminderId = ChatMessage.Split[2].ToInt();
            bool removed = DbControl.Reminders.Remove(ChatMessage.UserId, ChatMessage.Username, reminderId);
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

        Regex emotePattern = PatternCreator.Create(Alias, Prefix, @"\semote");
        if (emotePattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                Channel? channel = DbControl.Channels[ChatMessage.ChannelId];
                if (channel is null)
                {
                    Response += "an error occurred while trying to set the emote";
                    return;
                }

                channel.Emote = null;
                Response += "the emote has been unset";
            }
            else
            {
                Response += PredefinedMessages.NoModOrBroadcasterMessage;
            }

            return;
        }
    }
}
