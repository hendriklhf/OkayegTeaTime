using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class SetCommand : Command
{
    public SetCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\sprefix\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!ChatMessage.IsModerator && !ChatMessage.IsBroadcaster)
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
                return;
            }

            string prefix = ChatMessage.LowerSplit[2][..(ChatMessage.LowerSplit[2].Length > AppSettings.MaxPrefixLength
                ? AppSettings.MaxPrefixLength
                : ChatMessage.LowerSplit[2].Length)];
            Channel? channel = DbControl.Channels[ChatMessage.ChannelId];
            if (channel is null)
            {
                Response = $"{ChatMessage.Username}, an error occurred while trying to set the prefix";
                return;
            }

            channel.Prefix = prefix;
            Response = $"{ChatMessage.Username}, prefix set to: {prefix}";
            return;
        }

        pattern = PatternCreator.Create(_alias, _prefix, @"\semote\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                string emote = ChatMessage.Split[2][..(ChatMessage.Split[2].Length > AppSettings.MaxEmoteInFrontLength
                    ? AppSettings.MaxEmoteInFrontLength
                    : ChatMessage.Split[2].Length)];
                Channel? channel = DbControl.Channels[ChatMessage.ChannelId];
                if (channel is null)
                {
                    Response = $"{ChatMessage.Username}, an error occurred while trying to set the emote";
                    return;
                }

                channel.Emote = emote;
                Response = $"{ChatMessage.Username}, emote set to: {emote}";
            }
            else
            {
                Response += PredefinedMessages.NoModOrBroadcasterMessage;
            }

            return;
        }

        pattern = PatternCreator.Create(_alias, _prefix, @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (!ChatMessage.IsModerator && !ChatMessage.IsBroadcaster)
            {
                Response += "you have to be a mod or the broadcaster to set song request settings";
                return;
            }

            bool? state = null;
            if (Regex.IsMatch(ChatMessage.Split[2], @"(1|true|enabled?)"))
            {
                state = true;
            }
            else if (Regex.IsMatch(ChatMessage.Split[2], @"(0|false|disabled?)"))
            {
                state = false;
            }

            if (state is null)
            {
                Response += "the state can only be set to \"enabled\" or \"disabled\"";
                return;
            }

            SpotifyUser? user = DbControl.SpotifyUsers[ChatMessage.Channel];
            if (user is null)
            {
                Response += $"channel {ChatMessage.Channel} is not registered, they have to register first";
                return;
            }

            user.AreSongRequestsEnabled = state.Value;
            Response += $"song requests {(state.Value ? "enabled" : "disabled")} for channel {ChatMessage.Channel}";
            return;
        }

        pattern = PatternCreator.Create(_alias, _prefix, @"\slocation\s((private)|(public))\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string city = ChatMessage.Split[3..].JoinToString(' ');
            bool isPrivate = ChatMessage.LowerSplit[2][1] == 'r';
            User? user = DbControl.Users[ChatMessage.UserId];
            if (user is null)
            {
                user = new(ChatMessage.UserId, ChatMessage.Username)
                {
                    Location = city,
                    IsPrivateLocation = isPrivate
                };
                DbControl.Users.Add(user);
            }
            else
            {
                user.Location = city;
                user.IsPrivateLocation = isPrivate;
            }

            Response = $"{ChatMessage.Username}, your {(isPrivate ? "private" : "public")} location has been set";
        }
    }
}
