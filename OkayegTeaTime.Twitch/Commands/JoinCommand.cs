using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Join)]
public class JoinCommand : Command
{
    public JoinCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
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

            string channel = ChatMessage.LowerSplit[1];
            string response = _twitchBot.JoinChannel(channel[1..]);
            Response = $"{ChatMessage.Username}, {response}";
        }
    }
}
