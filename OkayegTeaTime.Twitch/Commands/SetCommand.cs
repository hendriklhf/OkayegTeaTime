using System.Text.RegularExpressions;
using HLE.Strings;
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
        Regex prefixPattern = PatternCreator.Create(Alias, Prefix, @"\sprefix\s\S+");
        if (prefixPattern.IsMatch(ChatMessage.Message))
        {
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
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
            }
            else
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
            }

            return;
        }

        Regex emotePattern = PatternCreator.Create(Alias, Prefix, @"\semote\s\S+");
        if (emotePattern.IsMatch(ChatMessage.Message))
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

        Regex songRequestPattern = PatternCreator.Create(Alias, Prefix, @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))");
        if (songRequestPattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (!ChatMessage.IsModerator && !ChatMessage.IsBroadcaster)
            {
                Response += "you have to be a mod or the broadcaster to set song request settings";
                return;
            }

            bool? state = null;
            if (ChatMessage.Split[2].IsMatch(@"(1|true|enabled?)"))
            {
                state = true;
            }
            else if (ChatMessage.Split[2].IsMatch(@"(0|false|disabled?)"))
            {
                state = false;
            }

            if (state is null)
            {
                Response += "the state can only be set to \"enabled\" or \"disabled\"";
                return;
            }

            bool set = DbController.SetSongRequestState(ChatMessage.Channel, state.Value);
            if (set)
            {
                Response += $"song requests {(state.Value ? "enabled" : "disabled")} for channel {ChatMessage.Channel}";
            }
            else
            {
                Response += $"channel {ChatMessage.Channel} is not registered, they have to register first";
            }
        }
    }
}
