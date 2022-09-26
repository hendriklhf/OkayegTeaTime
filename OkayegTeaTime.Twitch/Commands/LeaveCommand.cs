using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Leave)]
public sealed class LeaveCommand : Command
{
    public LeaveCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s#?\w{3,25}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!ChatMessage.IsBotModerator)
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoBotModerator}";
                return;
            }

            string channel = ChatMessage.LowerSplit[1];
            bool isValidChannel = StringHelper.FormatChannel(ref channel);
            if (!isValidChannel)
            {
                Response = $"{ChatMessage.Username}, the given channel is invalid";
                return;
            }

            bool isJoined = _twitchBot.Channels[channel] is not null;
            if (!isJoined)
            {
                Response = $"{ChatMessage.Username}, the bot is not connected to #{channel}";
                return;
            }

            bool success = _twitchBot.LeaveChannel(channel);
            Response = $"{ChatMessage.Username}, {(success ? "successfully left" : "failed to leave")} #{channel}";
        }
    }
}
