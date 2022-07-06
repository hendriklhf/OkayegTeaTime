using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class UnsetCommand : Command
{
    public UnsetCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\sprefix");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                Channel? channel = DbControl.Channels[ChatMessage.ChannelId];
                if (channel is null)
                {
                    Response = $"{ChatMessage.Username}, an error occurred while trying to set the prefix";
                    return;
                }

                channel.Prefix = null;
                Response += $"{ChatMessage.Username}, the prefix has been unset";
            }
            else
            {
                Response += PredefinedMessages.NoModOrBroadcasterMessage;
            }

            return;
        }

        pattern = PatternCreator.Create(_alias, _prefix, @"\sreminder\s\d+");
        if (pattern.IsMatch(ChatMessage.Message))
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

        pattern = PatternCreator.Create(_alias, _prefix, @"\semote");
        if (pattern.IsMatch(ChatMessage.Message))
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

        pattern = PatternCreator.Create(_alias, _prefix, @"\slocation");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            User? user = DbControl.Users[ChatMessage.UserId];
            if (user is null)
            {
                Response = $"{ChatMessage.Username}, you haven't set your location yet";
                return;
            }

            user.Location = null;
            Response = $"{ChatMessage.Username}, your location has been unset";
        }
    }
}
