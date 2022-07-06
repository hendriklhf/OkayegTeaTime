using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class LeaveCommand : Command
{
    public LeaveCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s#?\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!ChatMessage.IsBotModerator)
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoBotModerator}";
                return;
            }

            string channel = ChatMessage.LowerSplit[1].Remove("#");
            string response = _twitchBot.LeaveChannel(channel);
            Response = $"{ChatMessage.Username}, {response}";
        }
    }
}
