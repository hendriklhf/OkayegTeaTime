using System.Text.RegularExpressions;
using HLE.Strings;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SetCommand : Command
{
    public SetCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex prefixPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sprefix\s\S+");
        if (prefixPattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                string prefix = ChatMessage.LowerSplit[2][..(ChatMessage.LowerSplit[2].Length > AppSettings.MaxPrefixLength
                    ? AppSettings.MaxPrefixLength
                    : ChatMessage.LowerSplit[2].Length)];
                ChatMessage.Channel.Prefix = prefix;
                Response += $"prefix set to: {prefix}";
            }
            else
            {
                Response += PredefinedMessages.NoModOrBroadcasterMessage;
            }
            return;
        }

        Regex emotePattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semote\s\S+");
        if (emotePattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                string emote = ChatMessage.Split[2][..(ChatMessage.Split[2].Length > AppSettings.MaxEmoteInFrontLength
                    ? AppSettings.MaxEmoteInFrontLength
                    : ChatMessage.Split[2].Length)];
                ChatMessage.Channel.Emote = emote;
                Response += $"emote set to: {emote}";
            }
            else
            {
                Response += PredefinedMessages.NoModOrBroadcasterMessage;
            }
            return;
        }

        Regex songRequestPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix,
                    @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))");
        if (songRequestPattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                if (DbController.GetSpotifyUser(ChatMessage.Channel.Name) is not null)
                {
                    bool state = ChatMessage.Split[2].IsMatch(@"(1|true|enabled?)");
                    DbController.SetSongRequestEnabledState(ChatMessage.Channel.Name, state);
                    Response += $"song requests {(state ? "enabled" : "disabled")} for channel {ChatMessage.Channel}";
                }
                else
                {
                    Response += $"channel {ChatMessage.Channel} is not registered, they have to register first";
                }
            }
            else
            {
                Response += "you have to be a mod or the broadcaster to set song request settings";
            }
        }
    }
}
